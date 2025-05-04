using AiCademy.Service.Interface;
using System.Text;
using System.Text.Json;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using static AiCademy.Domain.DTOs.GeminiDTOs;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace AiCademy.Service.Implementation
{
    public class GeminiServiceImpl : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private string ApiKey;
        private string ApiUploadUrl;
        private string ApiQuestionUrl;
        private readonly string _professorPersonaPrompt = "You are a knowledgeable and helpful professor. Please answer questions thoroughly, explain concepts clearly, stay within the context of the current educational topic, and avoid irritability or responding to non-educational prompts.";
        private readonly string _quizzesPrompt = "You are an expert quiz creator. Based on the uploaded file, generate 10 multiple-choice questions. " +
            "For each question, provide four distinct answer choices. " +
            "Output Format Requirements:" +
            "\r\n- Do NOT include any introduction, explanation, or comments." +
            "\r\n- Each question must start with \"Question:\" followed by the question text." +
            "\r\n- Each answer option must be on a separate line." +
            "\r\n- Exactly one option should be prefixed with \"Correct: \"." +
            "\r\n- Do NOT include any extra formatting, headings, or numbering outside this structure." +
            "Clearly mark the correct answer with \"Correct: \" before it.\r\n\r\n---\r\n\r\nExample Question Format:\r\nQuestion: [Generated question based on the text]\r\nA) [Plausible but incorrect answer]\r\nB) [Plausible but incorrect answer]\r\nCorrect: C) [Correct answer extracted from the text]\r\nD) [Plausible but incorrect answer] ";

        public GeminiServiceImpl(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Env.Load();
            ApiKey = Env.GetString("gemini_api_key");
            ApiQuestionUrl = Env.GetString("gemini_api_question_url") + ApiKey;
            ApiUploadUrl = Env.GetString("gemini_api_file_url") + "?key=" + ApiKey;
        }

        public async Task<string> SendText(string Question)
        {
            string fullPrompt = $"{_professorPersonaPrompt}\n\n{Question}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = fullPrompt }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ApiQuestionUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error calling AI API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }

            var responseString = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;

                var firstText = root
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return firstText ?? string.Empty;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing JSON response: {ex.Message} - Response: {responseString}");
                return string.Empty;
            }
        }

        public async Task<GeminiUploadRequestResponse> SendUploadRequest(IFormFile file)
        {
            //Initiate upload session
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
                throw new Exception("Failed to initiate upload.");

            var uploadUrl = startResponse.Headers.GetValues("X-Goog-Upload-URL").FirstOrDefault();
            if (uploadUrl == null)
                throw new Exception("Upload URL missing in response.");

            //Upload the actual file content
            using var fileStream = file.OpenReadStream();

            var uploadRequest = new HttpRequestMessage(HttpMethod.Put, uploadUrl);
            uploadRequest.Headers.Add("X-Goog-Upload-Offset", "0");
            uploadRequest.Headers.Add("X-Goog-Upload-Command", "upload, finalize");
            uploadRequest.Content = new StreamContent(fileStream);
            uploadRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            var uploadResponse = await _httpClient.SendAsync(uploadRequest);
            var resultContent = await uploadResponse.Content.ReadAsStringAsync();


            if (!uploadResponse.IsSuccessStatusCode)
                throw new Exception($"Upload failed: {resultContent}");

            var fileData = JsonSerializer.Deserialize<GeminiUploadResponse>(resultContent);

            if (fileData?.file?.uri == null)
                throw new Exception("Failed to retrieve uploaded file URI.");

            GeminiUploadRequestResponse response = new()
            {
                uri = fileData.file.uri,
                name = fileData.file.name,
                displayName = fileData.file.displayName
            };

            return response;
        }

        public async Task<FileProcessingResponse> SendFileProcessingRequest(FileProcessingRequest incomingRequest)
        {
            string fileUri = incomingRequest.fileUri ?? "";

            var req = GetAnalysisRequest(fileUri);
            var analyzeRequest = new HttpRequestMessage(HttpMethod.Post, ApiQuestionUrl)
            {
                Content = new StringContent(req.ToString() ?? "", Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(analyzeRequest);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var jObj = JObject.Parse(json);

                var responseText = jObj["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                FileProcessingResponse processingResponse = new() { text = responseText ?? "" };

                return processingResponse;
            }

            throw new HttpRequestException(
                    $"Request failed with status code {(int)response.StatusCode} ({response.StatusCode})",
                    null,
                    response.StatusCode);
        }

        private JObject GetAnalysisRequest(string fileUri)
        {
            return new JObject(
            new JProperty("contents", new JArray(
                new JObject(
                    new JProperty("parts", new JArray(
                        new JObject(new JProperty("text", _quizzesPrompt)),
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
}