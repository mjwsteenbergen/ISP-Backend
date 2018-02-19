using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IEPBackendCrawler
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            //Task.Run(async () =>
            {
                try
                {
                    await PersonLoader.parsePerson(new Instructor { Email = "w.p.brinkman" });

                    List<Course> AllCourses = new List<Course>();

                    //Add all programmes
                    AllCourses.AddRange(await CourseLoader.Run("CS16-17.txt"));
                    AllCourses.AddRange(await CourseLoader.Run("CS17-18.txt"));


                    //Add to database
                    var database = new DelftContext();
                    database.Courses.AddRange(AllCourses);
                    database.Tags.AddRange(CourseLoader.tagCache);
                    database.Instructors.AddRange(SingleCourseParser.InstructorCache);

                    AllCourses.ForEach(i =>
                    {
                        i.Tags.ForEach(j =>
                        {
                            database.CourseToTag.Add(new CourseToTag
                            {
                                CourseCode = i.CourseCode,
                                TagName = j.TagName
                            });
                        });

                        i.OtherInstructors.ForEach(j =>
                        {
                            database.CourseToInstructor.Add(new CourseToInstructor
                            {
                                CourseCode = i.CourseCode,
                                Email = j.Email,
                                IsResposible = true
                            });
                        });

                        i.ResponsibleInstructor.ForEach(j =>
                        {
                            database.CourseToInstructor.Add(new CourseToInstructor
                            {
                                CourseCode = i.CourseCode,
                                Email = j.Email,
                                IsResposible = false
                            });
                        });
                    });

                    Console.WriteLine("Done writing courses");
                    System.Console.ReadKey();

                    var people = await PersonLoader.Run(AllCourses);
                    people.ForEach(i =>
                    {
                        database.Persons.Add(i);
                    });
                    database.SaveChanges();

                    Console.WriteLine("Done");
                    System.Console.ReadKey();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.InnerException?.Message ?? "No Inner exception");
                    Console.WriteLine(e.InnerException?.StackTrace ?? "No Inner Stacktrace");
                    Console.WriteLine();
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
            //).Wait();
            

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }
    }

    
}
