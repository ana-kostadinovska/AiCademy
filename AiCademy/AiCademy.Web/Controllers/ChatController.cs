using AiCademy.Service.Interface;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace AiCademy.Web.Controllers
{
    [Route("chat")]
    public class ChatController : Controller
    {
        private readonly IGeminiService _geminiService;

        public ChatController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] QuestionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("Question is required.");

            var answer = await _geminiService.SendText(request.Question);
            return Ok(new { response = answer });
        }
    }

    public class QuestionRequest
    {
        public string? Question { get; set; }
    }
}

