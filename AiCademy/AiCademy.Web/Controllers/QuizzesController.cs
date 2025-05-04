using AiCademy.Domain.DTOs;
using AiCademy.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using static AiCademy.Domain.DTOs.GeminiDTOs;

namespace AiCademy.Web.Controllers
{
    public class QuizzesController : Controller
    {
        private readonly IGeminiService _geminiService;

        public QuizzesController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadPdf(IFormFile file)
        {
            try
            {
                GeminiUploadRequestResponse response = await _geminiService.SendUploadRequest(file);
                return Ok(response);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("analyze-file")]
        public async Task<IActionResult> AnalyzeFile([FromBody] FileProcessingRequest incomingRequest)
        {
            try
            {
                FileProcessingResponse response = await _geminiService.SendFileProcessingRequest(incomingRequest);
                return Ok(response);
            } catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }
    }
}
