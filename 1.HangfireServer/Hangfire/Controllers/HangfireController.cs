namespace Hangfire.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    public partial class HangfireController(
       IConfiguration config,
       //IJobTaskService jobTaskService,
        IBackgroundJobClient backgroundJobClient
        ) : BaseController(config)
    {
        #region DI
        //private readonly IJobTaskService _jobTaskService = jobTaskService;
        private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;
        #endregion
    }
}
