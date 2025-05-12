using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCademy.Domain.DTOs
{
    public class QuizzDTOs
    {
        public class QuizResultDto
        {
            public string Question { get; set; }
            public string UserAnswer { get; set; }
            public string CorrectAnswer { get; set; }
            public int Points { get; set; }
        }

        public class QuizExportRequest
        {
            public List<QuizResultDto> Answers { get; set; }
        }

    }
}
