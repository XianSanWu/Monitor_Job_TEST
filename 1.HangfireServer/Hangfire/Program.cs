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
    .WriteTo.File("logs/Hangfire-Server-Init-.log", //���ͪ�log��r�ɡM�ɦW�OHangfire-Server-Init--log�}�Y
        rollingInterval: RollingInterval.Day,       //�C�Ѳ��s���ɮ�
        retainedFileCountLimit: 720                 //Log�O�d�ɶ�(24 hr * 30 Day=720)
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
        .Enrich.With(new LogEnricher()) //�Ȼs�ƨ�log���e
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        //.Enrich.WithProperty(ThreadNameEnricher.ThreadNamePropertyName, "MyDefaultThreadId")
        .CreateLogger();

    builder.Host.UseSerilog();
    #endregion

    #region Hangfire �]�w
    var hangfire_conn = builder.Configuration.GetConnectionString("Cdp");
#if DEBUG
    hangfire_conn = builder.Configuration.GetConnectionString("DefaultConnection");
#endif
    hangfire_conn = CryptoUtil.Decrypt(Base64Util.Decode(hangfire_conn ?? string.Empty), key, iv);

    builder.Services.AddHangfire(config =>
    {
        //var assemblies = new[] { typeof(JobAction).Assembly };

        // �]�w Hangfire �ϥΪ��ǦC�ƾ��P��x
        config.UseSimpleAssemblyNameTypeSerializer()
              .UseRecommendedSerializerSettings()
              .UseColouredConsoleLogProvider()
              .UseManagementPages(typeof(Program).Assembly, new ClientSideConfigurations());


        // �t�m����O��ܪ��׶q����
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

        // �t�m�x�s�覡�ASQL Server
        config.UseSqlServerStorage(hangfire_conn, new Hangfire.SqlServer.SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.FromSeconds(1), // �վ㦨��Ž���
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true // ���ɮį�
        });

    });

    builder.Services.AddHangfireServer(options =>
    {
        options.SchedulePollingInterval = TimeSpan.FromSeconds(1); // ��ť��Ƚ���
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
    #region �A�ȵ��U��k Scrutor��{�۰ʵ��U(Service + Repository Layer)
    /*
     * Singleton�G  ������ε{��(Process)�u�إߤ@�� Instance�A����ɭԳ��@�Υ�(�D�`�֥ΡA���������ܼ�)�C
     * Scoped�G     �b Request�� Response �e���B�z�L�{��������@�Τ@�� Instance�A�C�� Request ���|�إߤ@�ӷs����ҡC
     * Transien�G   �C���n�D����ɴN�إߤ@�ӷs���A�ä��@��(�֥ΡA���Ӹ귽)�C
    */
    //https://code-maze.com/dotnet-dependency-injection-with-scrutor/
    // ���]�Ҧ������f�M��{���b�P�@�ӵ{�Ƕ��ΩR�W�Ŷ���
    var servicesAssembly = typeof(ICommandDispatcher).Assembly;
    var repositoryAssembly = typeof(IUnitOfWork).Assembly;

    builder.Services.Scan(selector => selector
        .FromAssemblies(servicesAssembly, repositoryAssembly) // ���y Services �M�ת� Assembly 
        .AddClasses(
        //classes => classes
        //.Where(type => type != typeof(IUnitOfWork)
        //            && type != typeof(IDbHelper)
        //            && type != typeof(DbHelper)
        //            && type != typeof(UnitOfWork))  // �ư��o������
        )
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    );
    #endregion

    #region Session 10����
    builder.Services.AddDistributedMemoryCache();  // or another session provider
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(10);  // Set session timeout
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });
    #endregion

    // �[�J HttpContextAccessor �Φۭq���v Filter
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSingleton<DashboardBasicAuthorizationFilter>();

    #region CORS �]�w
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
        DashboardTitle = "�ʱ��t�Υ��ȱƵ{��",
        //AppPath = "/" // �^������
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
