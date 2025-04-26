using AiCademy.Domain.Identity;
using System.Collections.Generic;

namespace AiCademy.Domain.Models
{
    public class Course : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }

        // Наставник (FOREIGN KEY)
        public Guid? InstructorId { get; set; }
        public virtual ApplicationUser Instructor { get; set; }

        // Навигациски својства
        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; }
        public virtual ICollection<EnrolledCourse> Enrollments { get; set; }
        public virtual ICollection<Certificate> Certificates { get; set; }
        public virtual ICollection<ForumPost> ForumPosts { get; set; }
    }
}
