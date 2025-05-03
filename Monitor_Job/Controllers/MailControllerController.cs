using AutoMapper;
using Services.Interfaces;

namespace WebApi.Controllers
{
    public partial class MailControllerController(
       IConfiguration config,
       IMapper mapper,
       ILogger<MailControllerController> logger,
       IMailService mailService
       )
       : BaseController(config, mapper)
    {
        #region DI
        private readonly ILogger<MailControllerController> _logger = logger;
        private readonly IMailService _mailService = mailService;
        #endregion
    }
}
