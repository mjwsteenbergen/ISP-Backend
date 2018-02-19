using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IEPBackendCrawler
{
    public class Person
    {
        public string FullName { get; set; }
        public string Faculty { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Room { get; set; }
        public string ImageUrl { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }

    public class Course : IEquatable<Course>
    {
        public Course()
        {
            Tags = new List<Tag>();
            ResponsibleInstructor = new List<Instructor>();
            OtherInstructors = new List<Instructor>();
        }

        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public double ECTS { get; set; }

        public int QuarterStart { get; set; }
        public int QuarterLength { get; set; }
        public int ExamPeriod { get; set; }
        public int EducationPeriod { get; set; } //TODO string

        public string ContactHours { get; set; }

        [Column(TypeName = "text")]
        public string Contents { get; set; }

        [Column(TypeName = "text")]
        public string StudyGoals { get; set; }

        [Column(TypeName = "text")]
        public string Assessment { get; set; }

        [Column(TypeName = "text")]
        public string EducationMethod { get; set; }

        //Start Education
        //Education Period
        //Exam Period

        public string CourseYear { get; set; }
        [NotMapped]
        public List<Instructor> ResponsibleInstructor { get; set; }
        [NotMapped]
        public List<Instructor> OtherInstructors { get; set; }

        [NotMapped]
        public List<Tag> Tags { get; set; }
        public string Lang { get; internal set; }

        public override string ToString()
        {
            return CourseName;
        }



        public override int GetHashCode()
        {
            return CourseName.GetHashCode() ^ CourseYear.GetHashCode();
        }

        public bool Equals(Course other)
        {
            return CourseName == other.CourseName && CourseYear == other.CourseYear;
        }
    }

    public class Instructor
    {
        public string Email { get; set; }

        public override string ToString()
        {
            return Email;
        }
    }

    public class Tag : IEquatable<Tag>
    {
        public string TagName { get; set; }

        public override bool Equals(object obj)
        {
            if(obj is Tag)
            {
                return (obj as Tag).TagName == this.TagName;
            }
            return false;
        }

        public bool Equals(Tag other)
        {
            return other.TagName == TagName;
        }

        public override int GetHashCode()
        {
            return TagName == null ? 0 : TagName.GetHashCode(); ;
        }

        public override string ToString()
        {
            return TagName;
        }
    }

    public class CourseToTag
    {
        public string CourseCode { get; set; }
        public string TagName { get; set; }
    }

    public class CourseToInstructor
    {
        public string CourseCode { get; set; }
        public string Email { get; set; }
        public bool IsResposible { get; set; }
    }

    public class IEPCourse
    {
        Course c;

        public IEPCourse(Course course)
        {
            c = course;
        }
    }
}
