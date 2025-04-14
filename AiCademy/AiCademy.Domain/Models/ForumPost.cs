using AiCademy.Domain.Identity;
using System;

namespace AiCademy.Domain.Models
{
    public class ForumPost : BaseEntity
    {
        public string Content { get; set; }
        public DateTime PostDate { get; set; }

        // Релации
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Course Course { get; set; }
    }
}
