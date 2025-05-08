using AutoMapper;
using Services.Interfaces;

namespace WebApi.Controllers
{
    public partial class WorkflowStepsController(
       IConfiguration config,
       IMapper mapper,
       ILogger<WorkflowStepsController> logger,
       IMailService mailService,
       IWorkflowStepsService workflowStepsService,
       IFileService fileService,
       IMailhunterService mailhunterService

       )
       : BaseController(config, mapper)
    {
        #region DI
        private readonly ILogger<WorkflowStepsController> _logger = logger;
        private readonly IWorkflowStepsService _workflowStepsService = workflowStepsService;
        private readonly IFileService _fileService = fileService;
        private readonly IMailService _mailService = mailService;
        private readonly IMailhunterService _mailhunterService = mailhunterService;
        #endregion
    }
}

