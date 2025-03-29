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
    public class CourseServiceImpl : ICourseService
    {
        private readonly IRepository<Course> _courseRepository;

        public CourseServiceImpl(IRepository<Course> courseRepository)
        {
            _courseRepository = courseRepository;
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
