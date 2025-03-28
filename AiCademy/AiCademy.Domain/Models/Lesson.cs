using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCademy.Domain.Models
{
    public class Lesson : BaseEntity
    {
        public string Name { get; set; }
        public string? FilePath { get; set; }
        public Guid CourseId { get; set; }
        public Course? Course { get; set; }
    }
}
