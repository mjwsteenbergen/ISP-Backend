using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace IEPBackendCrawler
{
    public class DelftContext : DbContext
    {
        public DelftContext() : base() {
            var res = File.ReadAllLines("../IEP-Backend-Crawler/credentials");
            UserName = res[0];
            Password = res[1];
        }

        private readonly string UserName;
        private readonly string Password;

        public DbSet<Course> Courses { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<CourseToInstructor> CourseToInstructor { get; set; }
        public DbSet<CourseToTag> CourseToTag { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseSqlServer($"Server=tcp:tudelft.database.windows.net,1433;Initial Catalog=TU-Delft;Persist Security Info=False;User ID={UserName};Password={Password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>()
                .HasKey(c => new { c.CourseName, c.CourseYear });
            modelBuilder.Entity<Person>()
                            .HasKey(c => new { c.FullName });
            modelBuilder.Entity<Instructor>()
                .HasKey(c => new { c.Email });
            modelBuilder.Entity<Tag>()
                .HasKey(c => new { c.TagName });
            modelBuilder.Entity<CourseToInstructor>()
                .HasKey(c => new { c.CourseCode, c.Email, c.IsResposible});
            modelBuilder.Entity<CourseToTag>()
                .HasKey(c => new { c.CourseCode, c.TagName});
        }
    }

    public class DatabaseStorer
    {
        SqlConnection connection;

        public static void Main2()
        {
            using (var db = new DelftContext())
            {
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);

                Console.WriteLine();
                Console.WriteLine("All blogs in database:");
                foreach (var blog in db.Persons)
                {
                    Console.WriteLine(" - {0}", blog.FullName);
                }
            }
        }
    }
}
