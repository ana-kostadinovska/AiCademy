using System.Security.Policy;

namespace AiCademy.Web.Models
{
    public class Lesson : BaseEntity
    {
        public string Name { get; set; }
        public string? FilePath { get; set; }
        public Guid CourseId { get; set; }
        public Course? Course { get; set; }
    }
}
