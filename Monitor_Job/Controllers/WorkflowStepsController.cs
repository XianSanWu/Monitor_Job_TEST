using AutoMapper;
using Services.Interfaces;

namespace JobScheduling.Controllers
{
    public partial class WorkflowStepsController(
       IConfiguration config,
       IMapper mapper,
       ILogger<WorkflowStepsController> logger,
       IWorkflowStepsService workflowStepsService,
       IFileService fileService
       )
       : BaseController(config, mapper)
    {
        #region DI
        private readonly ILogger<WorkflowStepsController> _logger = logger;
        private readonly IWorkflowStepsService _workflowStepsService = workflowStepsService;
        private readonly IFileService _fileService = fileService;
        #endregion
    }
}

