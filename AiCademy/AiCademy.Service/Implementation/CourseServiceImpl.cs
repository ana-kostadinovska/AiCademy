using AiCademy.Domain.Models;
using AiCademy.Repository.Interface;
using AiCademy.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCademy.Service.Implementation
{
    public class CourseServiceImpl : ICourseService
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Lesson> _lessonRepository;

        public CourseServiceImpl(IRepository<Course> courseRepository, IRepository<Lesson> lessonRepository)
        {
            _courseRepository = courseRepository;
            _lessonRepository = lessonRepository;
        }

        public async Task AddLessonToCourse(Guid courseId, Lesson lesson)
        {
            var course = _courseRepository.Get(courseId);
            if (course == null)
            {
                throw new KeyNotFoundException("Course not found");
            }

            lesson.CourseId = courseId;
            _lessonRepository.Insert(lesson);

            course.Lessons.Add(lesson);
            _courseRepository.Update(course);
        }

        public Course CreateNewCourse(Course course)
        {
            return _courseRepository.Insert(course);
        }

        public Course DeleteCourse(Guid id)
        {
            var course = _courseRepository.Get(id);
            return _courseRepository.Delete(course);
        }

        public Course GetCourseById(Guid? id)
        {
            return _courseRepository.Get(id);
        }

        public List<Course> GetCourses()
        {
            return _courseRepository.GetAll().ToList();
        }

        public Course UpdateCourse(Course course)
        {
            return _courseRepository.Update(course);
        }
    }
}
