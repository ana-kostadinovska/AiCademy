using System;
using System.Collections.Generic;

namespace AiCademy.Domain.Models
{
    public class Lesson : BaseEntity
    {
        public string? Title { get; set; }
        public string? Content { get; set; }

        // Мултимедијални елементи
        public string? PresentationUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? ImageUrl { get; set; }

        // Релации
        public Guid? CourseId { get; set; }
        public virtual Course? Course { get; set; }

        // Метаподатоци
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        // Статус на лекцијата
        public bool? IsPublished { get; set; } = false;
    }
}
