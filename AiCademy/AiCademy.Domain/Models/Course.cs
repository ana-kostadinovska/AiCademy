using AiCademy.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCademy.Domain.Models
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
