using AutoMapper;
using Services.Interfaces;

namespace WebApi.Controllers
{
    public partial class MailhunterController(
       IConfiguration config,
       IMapper mapper,
       ILogger<MailhunterController> logger,
       IMailhunterService mailhunterService
       )
       : BaseController(config, mapper)
    {
        #region DI
        private readonly ILogger<MailhunterController> _logger = logger;
        private readonly IMailhunterService _mailhunterService = mailhunterService;
        #endregion
    }
}

