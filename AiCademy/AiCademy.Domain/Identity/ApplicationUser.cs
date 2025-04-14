using AiCademy.Domain.Enums;
using AiCademy.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCademy.Domain.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserType UserType { get; set; }

        public virtual ICollection<EnrolledCourse> EnrolledCourses { get; set; }
        public virtual ICollection<ForumPost> ForumPosts { get; set; }
        public virtual ICollection<Result> Results { get; set; }
        public virtual ICollection<Certificate> Certificates { get; set; }
    }
}
