using AiCademy.Domain.DTOs;
using AiCademy.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using static AiCademy.Domain.DTOs.GeminiDTOs;
using static AiCademy.Domain.DTOs.QuizzDTOs;

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

        [HttpPost("export-results")]
        public IActionResult ExportResults([FromBody] QuizExportRequest request)
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Quiz Results");

            worksheet.Cell(1, 1).Value = "Question";
            worksheet.Cell(1, 2).Value = "Your Answer";
            worksheet.Cell(1, 3).Value = "Correct Answer";
            worksheet.Cell(1, 4).Value = "Points";

            for (int i = 0; i < request.Answers.Count; i++)
            {
                var r = request.Answers[i];
                var row = i + 2;

                worksheet.Cell(row, 1).Value = r.Question;
                worksheet.Cell(row, 2).Value = r.UserAnswer;
                worksheet.Cell(row, 3).Value = r.CorrectAnswer;
                worksheet.Cell(row, 4).Value = r.Points;

                if (r.Points == 1)
                {
                    worksheet.Cell(row, 2).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGreen;
                    worksheet.Cell(row, 4).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGreen;
                }
                else
                {
                    worksheet.Cell(row, 2).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightPink;
                    worksheet.Cell(row, 4).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightPink;
                }
            }

            var totalRow = request.Answers.Count + 2;
            worksheet.Cell(totalRow, 1).Value = "Score";
            worksheet.Cell(totalRow, 4).Value = request.Answers.Sum(a => a.Points);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "QuizResults.xlsx");
        }

    }
}
