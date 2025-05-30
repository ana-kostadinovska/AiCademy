﻿using AiCademy.Domain.DTOs;
using AiCademy.Domain.Identity;
using AiCademy.Service.Interface;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static AiCademy.Domain.DTOs.GeminiDTOs;
using static AiCademy.Domain.DTOs.QuizzDTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;

namespace AiCademy.Web.Controllers
{
    public class QuizzesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGeminiService _geminiService;

        public QuizzesController(IGeminiService geminiService, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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

        [HttpPost("export-certificate")]
        public async Task<IActionResult> ExportCertificate()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in");

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found");

            // Create PDF document
            byte[] pdfBytes = CreateCertificatePdf(user.FirstName, user.LastName);

            return File(pdfBytes, "application/pdf", "Certificate.pdf");
        }

        private byte[] CreateCertificatePdf(string firstName, string lastName)
        {
            var fullName = $"{firstName} {lastName}";
            var currentDate = DateTime.Now.ToString("MMMM dd, yyyy");

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);
                    page.Background(Colors.White);

                    page.Content().Border(2).BorderColor(Colors.Grey.Medium).Padding(20).Column(col =>
                    {
                        col.Spacing(20);

                        col.Item().AlignCenter().Text("CERTIFICATE")
                            .FontSize(36)
                            .Bold()
                            .FontColor(Colors.Blue.Medium)
                            .FontFamily("Georgia")
                            .Underline();

                        col.Item().Height(10);

                        col.Item().AlignCenter().Text("This is to certify that")
                            .FontSize(14)
                            .Italic()
                            .FontColor(Colors.Grey.Darken1);

                        col.Item().AlignCenter().Text(fullName)
                            .FontSize(28)
                            .Bold()
                            .FontColor(Colors.Black);

                        col.Item().AlignCenter().Text("has successfully completed this lecture")
                            .FontSize(16)
                            .FontColor(Colors.Grey.Darken1);

                        col.Item().Height(30);

                        // Draw a horizontal line using bottom border on an empty container
                        col.Item().PaddingBottom(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(signatureCol =>
                            {
                                signatureCol.Item().Text("______________________").AlignCenter();
                                signatureCol.Item().Text("Instructor Signature").FontSize(12).FontColor(Colors.Grey.Darken2).AlignCenter();
                            });

                            row.RelativeItem().Column(dateCol =>
                            {
                                dateCol.Item().Text(currentDate).AlignCenter().FontSize(14);
                                dateCol.Item().Text("Date").FontSize(12).FontColor(Colors.Grey.Darken2).AlignCenter();
                            });
                        });
                    });
                });
            });

            try
            {
                return pdf.GeneratePdf();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PDF generation failed: {ex.Message}");
                throw;
            }
        }


    }
}
