using AiCademy.Domain.DTOs;
using Microsoft.AspNetCore.Http;

namespace AiCademy.Service.Interface
{
    public interface IGeminiService
    {
        public Task<string> SendText(string Question);

        public Task<GeminiDTOs.GeminiUploadRequestResponse> SendUploadRequest(IFormFile file);

        public Task<GeminiDTOs.FileProcessingResponse> SendFileProcessingRequest(GeminiDTOs.FileProcessingRequest incomingRequest);
    }

}
