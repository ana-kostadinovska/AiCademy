using AiCademy.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetEnv;

namespace AiCademy.Service.Implementation
{
    public class GeminiServiceImpl : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private string ApiKey;
        private string ApiQuestionUrl;
        private string ApiUploadUrl;

        public GeminiServiceImpl(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Env.Load();
            ApiKey = Env.GetString("gemini_api_key");
            ApiQuestionUrl = Env.GetString("gemini_api_question_url") + ApiKey;
            ApiUploadUrl = Env.GetString("gemini_api_file_url") + ApiKey;
        }

        public async Task<string> SendText(string Question)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = Question }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ApiQuestionUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error calling AI API");
            }

            var responseString = await response.Content.ReadAsStringAsync();

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
    }
}
