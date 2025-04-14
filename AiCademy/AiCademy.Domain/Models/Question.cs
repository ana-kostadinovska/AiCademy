using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCademy.Domain.Models
{
    public class Question : BaseEntity
    {
        public string Text { get; set; }

        public Guid QuizId { get; set; }
        public virtual Quiz Quiz { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
    }


}
