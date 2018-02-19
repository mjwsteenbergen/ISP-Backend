using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IEPBackendCrawler
{
    public class CourseLoader
    {
        public static async Task<List<Course>> Run(string programme)
        {
            string[] lines = System.IO.File.ReadAllLines(@"../IEP-Backend-Crawler/" + programme);
            
            List<Item> items = new List<Item>();
            foreach (string line in lines)
            {
                if (lines.First() == line) continue;
                // Use a tab to indent each line of the file.
                items.Add(ParseLine(line));
            }

            foreach (Item item in items)
            {
                var parent = items.Find(i => i.Id == item.ParentId);
                item.Parent = parent;
                parent?.Children?.Add(item);
            }

            int count = 0;
            while(items.Count > count) {
                Item item = items[count];
                int secondCount = count + 1;

                while(items.Count > secondCount)
                {
                    Item otherItem = items[secondCount];
                    if(otherItem.CourseName == item.CourseName)
                    {
                        Item parent = otherItem.Parent;
                        while (parent != null)
                        {
                            item.tags.Add(parent.CourseName);
                            parent = parent.Parent;
                        }
                        items.Remove(otherItem);
                    }
                    secondCount++;
                }
                count++;

            }



            List<Course> courses = new List<Course>();

            foreach (Item item in items)
            {
                if (item.Children.Count > 0) continue;

                if (courses.Any(i => i.CourseCode == item.CourseName)) continue;

                Course course = await SingleCourseParser.GetAllCourseInfo(item.CourseId.ToString());
                course.Tags = item.tags.Select(i => GetTagOrCreate(i)).Distinct().ToList();

                Item parent = item.Parent;
                while (parent != null)
                {
                    course.Tags.Add(GetTagOrCreate(parent.CourseName));
                    parent = parent.Parent;
                }
                course.Tags = course.Tags.Distinct().ToList();
                courses.Add(course);
            }

            return courses.Distinct().ToList();
        }

        public static Item ParseLine(string s)
        {
            s = s.Replace("d.add(", "");
            var items = s.Split(',');
            var newItem = new Item();
            newItem.Id = int.Parse(items[0]);
            newItem.ParentId = int.Parse(items[1]);
            newItem.CourseName = items[2];
            string value = Regex.Match(items[3], @"\((\d+)\)").Groups[1].Value;
            newItem.CourseId = int.Parse(value);
            return newItem;
        }


        public static HashSet<Tag> tagCache = new HashSet<Tag>();
        public static Tag GetTagOrCreate(string name)
        {
            var tag = tagCache.FirstOrDefault(i => i.TagName == FormatTagName(name));
            if(tag == null)
            {
                tag = new Tag { TagName = FormatTagName(name)};
                tagCache.Add(tag);
            }

            return tag;
        }

        public static string FormatTagName(string name) => name.Remove(0, 1).Remove(name.Length - 2, 1);
    }

    public class Item
    {
        public Item()
        {
            Children = new List<Item>();
        }

        public int Id { get; set; }
        public int ParentId { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }

        public Item Parent { get; set; }
        public List<Item> Children { get; set; }
        public HashSet<string> tags = new HashSet<string>();

        public override string ToString()
        {
            return CourseName;
        }
    }
}
