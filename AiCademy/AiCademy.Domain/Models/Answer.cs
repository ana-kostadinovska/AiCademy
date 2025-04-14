using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCademy.Domain.Models
{
    public class Answer : BaseEntity
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }

        public Guid QuestionId { get; set; }
        public virtual Question Question { get; set; }
    }


}
