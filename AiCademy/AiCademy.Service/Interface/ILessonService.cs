using AiCademy.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCademy.Service.Interface
{
    public interface ILessonService
    {
        public List<Lesson> GetLessons();
        public Lesson GetLessonById(Guid? id);
        public Lesson CreateNewLesson(Lesson lesson);
        public Lesson UpdateLesson(Lesson lesson);
        public Lesson DeleteLesson(Guid id);
    }
}
