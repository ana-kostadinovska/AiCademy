using AiCademy.Domain.Identity;
using System;

namespace AiCademy.Domain.Models
{
    public class Result : BaseEntity
    {
        public double Score { get; set; }
        public TimeSpan TimeTaken { get; set; }

        // Релации
        public string UserId { get; set; }
        public Guid QuizId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Quiz Quiz { get; set; }
    }
}
