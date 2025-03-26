using AiCademy.Web.Models.Identity;

namespace AiCademy.Web.Models
{
    public class Course : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public ICollection<Lesson>? Lessons { get; set; }
        public virtual ICollection<ApplicationUser>? Users { get; set; }
    }
}
