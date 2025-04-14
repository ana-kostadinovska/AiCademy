using AiCademy.Domain.Identity;
using AiCademy.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCademy.Repository
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Lesson> Lessons { get; set; }
        public virtual DbSet<EnrolledCourse> EnrolledCourses { get; set; }
        public virtual DbSet<Quiz> Quizzes { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<ForumPost> ForumPosts { get; set; }
        public virtual DbSet<Certificate> Certificates { get; set; }
        public virtual DbSet<Result> Results { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
