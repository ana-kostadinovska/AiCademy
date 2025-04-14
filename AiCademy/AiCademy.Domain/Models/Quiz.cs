using System.Collections.Generic;

namespace AiCademy.Domain.Models
{
    public class Quiz : BaseEntity
    {
        public string Name { get; set; }

        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }

}
