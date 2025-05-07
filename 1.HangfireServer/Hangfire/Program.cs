using Hangfire;
using Hangfire.Dashboard.Management.v2;
using Hangfire.Dashboard.Management.v2.Support;
using Hangfire.Extensions;
using Hangfire.Filters;
using Hangfire.Jobs;
using Hangfire_Repository.Interfaces;
using Hangfire_Servies.Interfaces;
using Hangfire_Utilities.Utilities;
using Serilog;
using Serilog.Events;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()                              // This line requires the Serilog.Sinks.Console package
    .WriteTo.File("logs/Hangfire-Server-Init-.log", //產生的log文字檔﹐檔名是Hangfire-Server-Init--log開頭
        rollingInterval: RollingInterval.Day,       //每天產新的檔案
        retainedFileCountLimit: 720                 //Log保留時間(24 hr * 30 Day=720)
    )
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

try
{
    var key = builder.Configuration["EncryptionSettings:AESKey"] ?? string.Empty;
    var iv = builder.Configuration["EncryptionSettings:AESIV"] ?? string.Empty;

    Log.Information("Starting Hangfire-Server host");

    #region Serilog
    //  Read Serilog config from appsettings.json (https://blog.miniasp.com/post/2021/11/29/How-to-use-Serilog-with-NET-6)
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.With(new LogEnricher()) //客製化取log內容
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        //.Enrich.WithProperty(ThreadNameEnricher.ThreadNamePropertyName, "MyDefaultThreadId")
        .CreateLogger();

    builder.Host.UseSerilog();
    #endregion

    #region Hangfire 設定
    var hangfire_conn = builder.Configuration.GetConnectionString("Cdp");
#if DEBUG
    hangfire_conn = builder.Configuration.GetConnectionString("DefaultConnection");
#endif
    hangfire_conn = CryptoUtil.Decrypt(Base64Util.Decode(hangfire_conn ?? string.Empty), key, iv);

    builder.Services.AddHangfire(config =>
    {
        //var assemblies = new[] { typeof(JobAction).Assembly };

        // 設定 Hangfire 使用的序列化器與日誌
        config.UseSimpleAssemblyNameTypeSerializer()
              .UseRecommendedSerializerSettings()
              .UseColouredConsoleLogProvider()
              .UseManagementPages(typeof(Program).Assembly, new ClientSideConfigurations());


        // 配置儀表板顯示的度量指標
        config.UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.ServerCount)
              .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.RecurringJobCount)
              .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.RetriesCount)
              .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.EnqueuedAndQueueCount)
              .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.ScheduledCount)
              .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.ProcessingCount)
              .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.SucceededCount)
              .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.FailedCount)
              .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.DeletedCount)
              .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.AwaitingCount);

        // 配置儲存方式，SQL Server
        config.UseSqlServerStorage(hangfire_conn, new Hangfire.SqlServer.SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.FromSeconds(1), // 調整成秒級輪詢
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true // 提升效能
        });

    });

    builder.Services.AddHangfireServer(options =>
    {
        options.SchedulePollingInterval = TimeSpan.FromSeconds(1); // 秒級任務輪詢
        options.WorkerCount = Environment.ProcessorCount * 2;

        var queues = new List<string>
        {
            "default"
        };
        /*
            See note about JobsHelper.GetAllQueues()
            under the 'Defining Jobs' section below
        */
        queues.AddRange(JobsHelper.GetAllQueues());

        options.Queues = queues.Distinct()?.ToArray();
    });

    builder.Services.AddScoped<JobExecutor>();

    #endregion

    // Add services to the container.
    #region 服務註冊方法 Scrutor實現自動註冊(Service + Repository Layer)
    /*
     * Singleton：  整個應用程式(Process)只建立一個 Instance，任何時候都共用它(非常少用，類似全域變數)。
     * Scoped：     在 Request到 Response 前的處理過程執行期間共用一個 Instance，每個 Request 都會建立一個新的實例。
     * Transien：   每次要求元件時就建立一個新的，永不共用(少用，較耗資源)。
    */
    //https://code-maze.com/dotnet-dependency-injection-with-scrutor/
    // 假設所有的接口和實現都在同一個程序集或命名空間中
    var servicesAssembly = typeof(ICommandDispatcher).Assembly;
    var repositoryAssembly = typeof(IUnitOfWork).Assembly;

    builder.Services.Scan(selector => selector
        .FromAssemblies(servicesAssembly, repositoryAssembly) // 掃描 Services 專案的 Assembly 
        .AddClasses(
        //classes => classes
        //.Where(type => type != typeof(IUnitOfWork)
        //            && type != typeof(IDbHelper)
        //            && type != typeof(DbHelper)
        //            && type != typeof(UnitOfWork))  // 排除這些類型
        )
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    );
    #endregion

    #region Session 10分鐘
    builder.Services.AddDistributedMemoryCache();  // or another session provider
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(10);  // Set session timeout
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });
    #endregion

    // 加入 HttpContextAccessor 及自訂授權 Filter
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSingleton<DashboardBasicAuthorizationFilter>();

    #region CORS 設定
    var AllowMyFrontEnd = "AllowMyFrontEnd";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: AllowMyFrontEnd,
            policy =>
            {
                // policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader(); 

                var allowCors = (builder.Configuration["AppConfig:Cors"] ?? string.Empty).Split(",");
                policy.WithOrigins(allowCors).AllowAnyMethod().AllowAnyHeader();
            });
    });
    #endregion

    builder.Services.AddHttpClient();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseSession();

    var dashboardOptions = new DashboardOptions
    {
        Authorization = [app.Services.GetRequiredService<DashboardBasicAuthorizationFilter>()],
        DashboardTitle = "監控系統任務排程器",
        //AppPath = "/" // 回首頁用
    };

    app.UseHangfireDashboard("/hangfire", dashboardOptions);

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors(AllowMyFrontEnd);

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

    Log.Information("Hangfire-Server host started successfully");

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
