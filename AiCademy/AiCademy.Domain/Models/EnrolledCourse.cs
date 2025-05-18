using AiCademy.Domain.Enums;
using AiCademy.Domain.Identity;
using System;

namespace AiCademy.Domain.Models
{
    public class EnrolledCourse : BaseEntity
    {
        // Релации со корисник и курс
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }

        // Статус на запишувањето
        public CourseStatus Status { get; set; }

        // Датуми на запишување и завршување
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
