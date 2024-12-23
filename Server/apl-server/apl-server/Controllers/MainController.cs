using apl_server.Request;
using apl_server.Service;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace apl_server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MainController> _logger;

        public MainController(ILogger<MainController> logger, IMessageService messageService)
        {
            _logger = logger;
            _messageService = messageService;
        }

        [HttpPost("Messages")]
        public async Task<IActionResult> Post(Message request)
        {
            var responseMessage = await _messageService.ProcessaMensagem(request);
            return Ok(responseMessage);
        }
    }
}
