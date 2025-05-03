using AiCademy.Service.Interface;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Newtonsoft.Json.Linq;
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
        private string ApiUploadUrl;
        private string ApiQuestionUrl;
        private string fileUri;

        public AIController(HttpClient httpClient, IGeminiService geminiService)
        {
            _httpClient = httpClient;
            Env.Load();
            ApiKey = Env.GetString("gemini_api_key");
            ApiUploadUrl = Env.GetString("gemini_api_file_url") + "?key=" + ApiKey;
            ApiQuestionUrl = Env.GetString("gemini_api_question_url") + ApiKey;
            _geminiService = geminiService;
            fileUri = "";
        }

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadPdf(IFormFile file)
        {
            // STEP 1: Initiate upload session
            var startRequest = new HttpRequestMessage(HttpMethod.Post, ApiUploadUrl);
            startRequest.Headers.Add("X-Goog-Upload-Protocol", "resumable");
            startRequest.Headers.Add("X-Goog-Upload-Command", "start");
            startRequest.Headers.Add("X-Goog-Upload-Header-Content-Type", "application/pdf");
            startRequest.Content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    file = new { display_name = file.FileName }
                }), Encoding.UTF8, "application/json");

            var startResponse = await _httpClient.SendAsync(startRequest);
            if (!startResponse.IsSuccessStatusCode)
                return BadRequest("Failed to initiate upload.");

            var uploadUrl = startResponse.Headers.GetValues("X-Goog-Upload-URL").FirstOrDefault();
            if (uploadUrl == null)
                return BadRequest("Upload URL missing in response.");

            // STEP 2: Upload the actual file content
            using var fileStream = file.OpenReadStream();

            var uploadRequest = new HttpRequestMessage(HttpMethod.Put, uploadUrl);
            uploadRequest.Headers.Add("X-Goog-Upload-Offset", "0");
            uploadRequest.Headers.Add("X-Goog-Upload-Command", "upload, finalize");
            uploadRequest.Content = new StreamContent(fileStream);
            uploadRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            var uploadResponse = await _httpClient.SendAsync(uploadRequest);
            var resultContent = await uploadResponse.Content.ReadAsStringAsync();


            if (!uploadResponse.IsSuccessStatusCode)
                return BadRequest($"Upload failed: {resultContent}");

            var fileData = JsonSerializer.Deserialize<GeminiUploadResponse>(resultContent);

            if (fileData?.file?.uri == null)
                return BadRequest("Failed to retrieve uploaded file URI.");

            fileUri = fileData.file.uri;

            return Ok(new
            {
                uri = fileUri,
                name = fileData.file.name,
                displayName = fileData.file.displayName
            });
        }

        [HttpPost("analyze-file")]
        public async Task<IActionResult> AnalyzeFile([FromBody] AnalyzeRequest aReq) 
        {
            string text = aReq.text;
            var req = GetAnalysisRequest(text);
            var analyzeRequest = new HttpRequestMessage(HttpMethod.Post, ApiQuestionUrl)
            {
                Content = new StringContent(req.ToString(), Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(analyzeRequest);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return Ok(responseContent);
            }

            return StatusCode((int)response.StatusCode, "Request failed");

        }

        public object GetAnalysisRequest(string text)
        {
            return new JObject(
            new JProperty("contents", new JArray(
                new JObject(
                    new JProperty("parts", new JArray(
                        new JObject(new JProperty("text", text)),
                        new JObject(
                            new JProperty("file_data", new JObject(
                                new JProperty("mime_type", "application/pdf"),
                                new JProperty("file_uri", fileUri)
                            ))
                        )
                    ))
                )
            ))
        );
        }
    }

    public class UploadRequest
    {
        public DisplayName file { get; set; }
    }

    public class DisplayName
    {
        [System.Text.Json.Serialization.JsonPropertyName("display_name")]
        public string display_name { get; set; }
    }

    public class GeminiUploadResponse
    {
        public GeminiFile file { get; set; }
    }

    public class GeminiFile
    {
        public string name { get; set; }
        public string displayName { get; set; }
        public string mimeType { get; set; }
        public string uri { get; set; }
    }

    public class AnalyzeRequest
    {
        public string text { get; set; }
    }
}
