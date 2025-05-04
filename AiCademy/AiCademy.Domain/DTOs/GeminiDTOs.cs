using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCademy.Domain.DTOs
{
    public class GeminiDTOs
    {
        //File request models
        public class GeminiUploadResponse
        {
            public GeminiFile file { get; set; }
        }

        public class GeminiFile
        {
            public string? name { get; set; }
            public string? displayName { get; set; }
            public string? mimeType { get; set; }
            public string? uri { get; set; }
        }

        public class GeminiUploadRequestResponse
        {
            public string? uri { get; set; }
            public string? name { get; set; }
            public string? displayName { get; set; }
        }

        //Process file models

        public class FileProcessingRequest
        {
            public string? fileUri { get; set; }
        }

        public class FileProcessingResponse
        {
            public string? text { get; set; }
        }
    }
}
