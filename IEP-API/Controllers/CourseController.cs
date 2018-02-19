using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IEPBackendCrawler;
using Microsoft.AspNetCore.Mvc;

namespace IEP_API.Controllers
{
    [Route("api/course")]
    public class CourseController : Controller
    {
        // GET api/values
        [HttpGet]
        public async Task<JsonResult> GetAsync()
        {
            DelftContext context = new DelftContext();

            foreach(var course in context.Courses)
            {
                course.Tags = context.CourseToTag.Where(i => i.CourseCode == course.CourseCode).Select(i => new Tag { TagName = i.TagName }).ToList();
            }

            return Json(context.Courses);
        }

        // GET api/values/5
        [HttpGet("{courseId}")]
        public async Task<JsonResult> GetAsync(string courseId)
        {
            DelftContext context = new DelftContext();

            var course = context.Courses.FirstOrDefault(i => i.CourseCode == courseId);

            var courseTags = context.CourseToTag.Where(i => i.CourseCode == courseId);
            var course2Instructors = context.CourseToInstructor.Where(i => i.CourseCode == courseId);

            course.Tags = context.Tags.Where(i => courseTags.Any(j => j.TagName == i.TagName)).ToList();
            course.ResponsibleInstructor = course2Instructors.Where(i => !i.IsResposible).Select(i => new Instructor { Email = i.Email }).ToList();
            course.OtherInstructors = course2Instructors.Where(i => i.IsResposible).Select(i => new Instructor { Email = i.Email }).ToList();
            //course.Tags = context.Tags.ToList();
            

            //IEPCourse newC = (IEPCourse)course;
            //newC.CourseTags = newC.Tags.Select(i => i.TagName).ToList();
            //newC.ResponsibleInstructor = course2Instructors.Where(i => i.IsResposible).Select(i => context.Persons.FirstOrDefault(j => j.Email == i.Email)).ToList();
            //newC.OtherInstructors = course2Instructors.Where(i => !i.IsResposible).Select(i => context.Persons.FirstOrDefault(j => j.Email == i.Email)).ToList();
            return Json(course);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
