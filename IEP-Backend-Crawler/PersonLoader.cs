using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEPBackendCrawler
{
    public class PersonLoader
    {
        internal static async Task<List<Person>> Run(List<Course> courses)
        {
            var instructors = courses.Select(i => i.OtherInstructors.Union(i.ResponsibleInstructor)).SelectMany(i => i).Distinct().ToList();

            return instructors.AsParallel().Select(async instructor => await parsePerson(instructor)).Select(async i => await i).Select(i => i.Result).ToList();
        }


        public static async Task<Person> parsePerson(Instructor email)
        {
            var personId = email.Email.Replace("@tudelft.nl", "");

            HtmlDocument doc = await (new HtmlWeb().LoadFromWebAsync("https://www.tudelft.nl/en/staff/" + personId));
            Console.WriteLine(doc.ParsedText);
            Dictionary<string, HtmlNode> key = new Dictionary<string, HtmlNode>();
            Person newPerson = new Person();
            newPerson.FullName = personId;
            newPerson.Email = email.Email;
            try
            {

                newPerson.FullName = Format(doc.DocumentNode.Descendants("h1").First().InnerText);

                var allDiv = doc.DocumentNode.Descendants("div");
                var profileBox = allDiv.FirstOrDefault(i => i.Attributes.Any(j => j.Name == "class" && j.Value == "profile theme-blue"));
                if (profileBox == null)
                {
                    profileBox = doc.DocumentNode.SelectSingleNode("/html/body/main/div/div/aside");
                }

                var image = profileBox.Descendants("img").FirstOrDefault();
                if (image != null)
                {
                    newPerson.ImageUrl = image.Attributes.First(i => i.Name == "src").Value;
                }

                var allLinks = profileBox.Descendants("a");

                var telephone = allLinks.FirstOrDefault(i => i.Attributes.Any(j => j.Value.StartsWith("tel")));

                if(telephone != null)
                {
                    newPerson.Telephone = telephone.Attributes.First(i => i.Name == "href").Value;
                }

                newPerson.Faculty = Format(profileBox.Descendants("li").Last().InnerText);

                newPerson.Room = Format(profileBox.Descendants("i").First(i => i.Attributes.Any(j => j.Value == "i-map")).ParentNode.InnerText);

            } catch(Exception e)
            {
            }

            return newPerson;

        }

        public static string Format(string s)
        {
            return s.Replace("\t", "").Replace("\r", "").Replace("\n", "");
        }
    }

    
}
