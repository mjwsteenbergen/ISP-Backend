using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IEPBackendCrawler
{
    public class SingleCourseParser
    {
        public static async Task<Course> GetAllCourseInfo(string courseId, int times = 0)
        {
            Course course = new Course();
            HtmlDocument doc = await (new HtmlWeb().LoadFromWebAsync("http://studiegids.tudelft.nl/a101_displayCourse.do?course_id=" + courseId));
            Console.WriteLine(doc.ParsedText);
            Dictionary<string, HtmlNode> key = new Dictionary<string, HtmlNode>();

            try
            {
                //Console.WriteLine(current.CourseName);
                var table = doc.DocumentNode.Descendants("table").ToList().First(i => i.Attributes.Any(j => j.Value == "sisBody") && i.Attributes.Any(j => j.Value == "4"));
                //Console.WriteLine(table.InnerHtml);
                //var instructor = table.Descendants("table").First(i => i.Attributes.Any(j => j.Value == "sisbody"));


                foreach (var tableRow in table.ChildNodes.Where(i => i.Name == "tr"))
                {
                    var columns = tableRow.ChildNodes.Where(i => i.Name == "td");
                    key.Add(Format(columns.ElementAt(0).InnerText), columns.ElementAt(1));
                }


                if (key.ContainsKey("Contact Hours / Week  x/x/x/x")) {
                    course.ContactHours = Format(key.First(i => i.Key.Contains("Contact Hours ")).Value.InnerText);
                }

                if (key.ContainsKey("Course Language"))
                {
                    course.Lang = Format(key.First(i => i.Key.Contains("Course Language")).Value.InnerText);
                }

                if(key.ContainsKey("Responsible Instructor"))
                {
                    course.ResponsibleInstructor = key.First(i => i.Key.Contains("Responsible Instructor")).Value.Descendants().Where(i => i.Name == "a" && i.Attributes.Any(j => j.Name == "href" && j.Value.StartsWith("mailto"))).Select(i=> GetInstructorOrCreate(Format(i.InnerText))).ToList();
                }

                if (key.ContainsKey("Instructor"))
                {
                    course.OtherInstructors = key.First(i => Format(i.Key) == "Instructor").Value.Descendants().Where(i => i.Name == "a" && i.Attributes.Any(j => j.Name == "href" && j.Value.StartsWith("mailto"))).Select(i => GetInstructorOrCreate(Format(i.InnerText))).ToList();
                }
                //responsible instructors":"A. Bacchelli (A.Bacchelli@tudelft.nl)","instructors":"","coInstructors

                if (key.ContainsKey("Course Contents"))
                {
                    course.Contents = Format(key.GetValueOrDefault("Course Contents").ChildNodes[1].InnerHtml);
                }

                if (key.ContainsKey("Study Goals"))
                {   
                    course.StudyGoals = Format(key.GetValueOrDefault("Study Goals").ChildNodes[1].InnerHtml);
                }

                if (key.ContainsKey("Assessment"))
                {
                    course.Assessment = Format(key.GetValueOrDefault("Assessment").ChildNodes[1].InnerHtml);
                }

                if (key.ContainsKey("Start Education"))
                {
                    int.TryParse(key.First(i => Format(i.Key).Contains("Start Education")).Value.InnerText, out int res);
                    course.QuarterStart = res;
                }

                if (key.ContainsKey("Education Method"))
                {
                    course.EducationMethod = Format(key.GetValueOrDefault("Education Method").InnerText);
                }

                if (key.ContainsKey("Education Period"))
                {
                    //course.EducationPeriod = Regex.Replace(Format(key.GetValueOrDefault("Education Period").InnerText), @"(\n|\r| )+", ", ");
                }

                if (key.ContainsKey("Exam Period"))
                {
                    int.TryParse(Format(key.GetValueOrDefault("Exam Period").InnerText), out int res);
                    course.ExamPeriod = res;
                }

                //Expected knowledge
                //Judgement
                //Parts
                //Remarks
                //prerequisites

                var aboutTable = doc.DocumentNode.Descendants("table").First(i => i.Attributes.Any(j => j.Value == "sisBody") && i.Attributes.Any(j => j.Value == "100%"));

                var items = aboutTable.Descendants("td");

                course.CourseName = items.ElementAt(7).InnerText;
                course.CourseCode = items.ElementAt(6).InnerText;
                course.CourseYear = items.ElementAt(2).InnerText;
                course.ECTS = double.Parse(items.ElementAt(8).InnerText.Replace("ECTS: ", ""));
            } catch(Exception e)
            {
                if(times > 5)
                {
                    throw e;
                }
                Console.WriteLine("Something broke");
                await Task.Delay(500);
                return await GetAllCourseInfo(courseId, times++);
            }
            Console.Clear();
            return course;

        }

        public static HashSet<Instructor> InstructorCache = new HashSet<Instructor>();
        public static Instructor GetInstructorOrCreate(string email)
        {
            var instructor = InstructorCache.FirstOrDefault(i => i.Email == email);
            if (instructor == null)
            {
                instructor = new Instructor { Email = email };
                InstructorCache.Add(instructor);
            }

            return instructor;
        }

        public static string Format(string s)
        {
            s = s.Replace("<br>", "\n");
            return Regex.Match(s, @"([^\s\n\r](.|\n|\r)*[^\s\n\r]|[^\s\n\r])").Groups[1].Value;
        }
    }
}
