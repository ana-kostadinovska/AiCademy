using AiCademy.Domain.Models;
using AiCademy.Repository.Interface;
using AiCademy.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCademy.Service.Implementation
{
    public class LessonServiceImpl : ILessonService
    {
        private readonly IRepository<Lesson> _lessonRepository;

        public LessonServiceImpl(IRepository<Lesson> lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        public Lesson CreateNewLesson(Lesson lesson)
        {
            return _lessonRepository.Insert(lesson);
        }

        public Lesson DeleteLesson(Guid id)
        {
            var lesson = _lessonRepository.Get(id);
            return _lessonRepository.Delete(lesson);
        }

        public Lesson GetLessonById(Guid? id)
        {
            return _lessonRepository.Get(id);
        }

        public List<Lesson> GetLessons()
        {
            return _lessonRepository.GetAll().ToList();
        }

        public Lesson UpdateLesson(Lesson lesson)
        {
            return _lessonRepository.Update(lesson);
        }
    }
}
