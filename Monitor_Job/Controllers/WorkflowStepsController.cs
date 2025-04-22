using AutoMapper;
using Services.Interfaces;

namespace JobScheduling.Controllers
{
    public partial class WorkflowStepsController(
       IConfiguration config,
       IMapper mapper,
       ILogger<WorkflowStepsController> logger,
       //WorkflowStepsSearchListRequestValidator searchListRequestValidator,
       IWorkflowStepsService workflowStepsService,
       IFileService fileService
       //IConsumer<Ignore, Ignore> consumer,
       //string topic
       )
       : BaseController(config, mapper)
    {
        #region DI
        //private readonly IConsumer<Ignore, Ignore> _consumer = consumer;
        //private readonly string _topic = topic;
        private readonly ILogger<WorkflowStepsController> _logger = logger;
        //private readonly WorkflowStepsSearchListRequestValidator _searchListRequestValidator = searchListRequestValidator;
        private readonly IWorkflowStepsService _workflowStepsService = workflowStepsService;
        private readonly IFileService _fileService = fileService;
        #endregion
    }
}

