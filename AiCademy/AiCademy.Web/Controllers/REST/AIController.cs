using AiCademy.Service.Interface;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AiCademy.Web.Controllers.REST
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IGeminiService _geminiService;
        private string ApiKey;
        private string ApiQuestionUrl;
        private string ApiUploadUrl;

        public AIController(HttpClient httpClient, IGeminiService geminiService)
        {
            _httpClient = httpClient;
            //Env.Load();
            //ApiKey = Env.GetString("gemini_api_key");
            //ApiQuestionUrl = Env.GetString("gemini_api_question_url") + ApiKey;
            //ApiUploadUrl = Env.GetString("gemini_api_file_url") + ApiKey;
            ApiKey = "";
            ApiQuestionUrl = "";
            ApiUploadUrl = "";
            _geminiService = geminiService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskAI([FromBody] AIRequest request)
        {
            try
            {
                var result = await _geminiService.SendText(request.Question);
                return Ok(result);
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Doesn't work - Could be reworked with a python service
        [HttpPost("upload-pdf")]
        public async Task<IActionResult> UploadPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!file.ContentType.Equals("application/pdf") ||
                !Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only PDF files are allowed.");

            try
            {
                using var formData = new MultipartFormDataContent();


                // 1. Add metadata
                var metadataObj = new
                {
                    displayName = file.FileName,
                    mimeType = "application/pdf"
                };
                var metadataJson = JsonSerializer.Serialize(metadataObj);

                var metadataContent = new StringContent(metadataJson, Encoding.UTF8, "application/json");
                metadataContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"metadata\""
                };
                formData.Add(metadataContent);

                // 2. Add file
                using var fileStream = file.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                fileContent.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("form-data")
                {
                        Name = "\"file\"",
                        FileName = "\"" + file.FileName + "\""
                    };
                formData.Add(fileContent);

                // 3. Upload to Gemini
                var response = await _httpClient.PostAsync(ApiUploadUrl, formData);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Gemini API Error {errorContent}");
                    return StatusCode((int)response.StatusCode, errorContent); // Return actual error
                }

                // 4. Parse response
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<UploadResponse>(responseJson);

                if (result?.File?.Name == null)
                    return StatusCode(500, "Upload succeeded but no file ID was returned.");

                return Ok(new { fileId = result.File.Name });
            }
            catch (Exception ex)
            {
                Console.WriteLine("PDF Upload failed! ---- " + ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("analyze-file")]
        public async Task<IActionResult> AnalyzeFile([FromBody] AnalyzeRequest request)
        {
            if (string.IsNullOrEmpty(request.FileId)) // Changed from FileName to FileId
            {
                return BadRequest("File ID is required.");
            }

            try
            {
                // Gemini expects the file reference in a specific format
                var requestBody = new
                {
                    contents = new[]
                    {
                new
                {
                    parts = new object[]
                    {
                        new { text = "Analyze this document and provide key insights." }, // Prompt
                        new {
                            fileData = new  // Changed from file_data to fileData
                            {
                                mimeType = "application/pdf",
                                fileUri = request.FileId // Should be in format "files/your-file-id"
                            }
                        }
                    }
                }
            }
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(ApiQuestionUrl, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorContent);
                }

                var responseString = await response.Content.ReadAsStringAsync();
                return Ok(responseString);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        public class AnalyzeRequest
        {
            public string FileId { get; set; }
        }

        public class AIRequest
        {
            public string Question { get; set; }
        }

        public class UploadResponse
        {
            public FileObject File { get; set; }
        }

        public class FileObject
        {
            public string Name { get; set; }  // "files/abc123"
        }
    }
}
