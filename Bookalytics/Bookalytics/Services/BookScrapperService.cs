using AngleSharp;
using Bookalytics.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bookalytics.Services
{
    public class BookScrapperService:IBookScrapperService
    {
        private const string BaseUrl = "https://chitanka.info/text/{0}/0";

        private readonly IBrowsingContext context;

        public BookScrapperService()
        {
            var config = Configuration.Default.WithDefaultLoader();
            this.context = BrowsingContext.New(config);
        }

        //BIG TO-DO!

        public async Task Main()
        {
            for (int i = 0; i < 1; i++)
            {
                var url = $"https://chitanka.info/text/{24967}/0";
                var document = await context.OpenAsync(url);

                Console.WriteLine(new string('=', 50));
                Console.WriteLine(i);
                var category = document
                    .QuerySelectorAll(".bookcat")
                    .FirstOrDefault();

                var author = document.QuerySelectorAll(".bookauthor").FirstOrDefault();

                var year = document
                    .QuerySelectorAll("span")
                    .Where(x => x.GetAttribute("itemProp") == "datePublished")
                    .FirstOrDefault();

                var title = document.QuerySelectorAll("i")
                    .Where(x => x.GetAttribute("itemProp") == "name")
                    .FirstOrDefault();


                var imgUrl = document.QuerySelectorAll("img")
                     .Where(x => x.GetAttribute("itemProp") == "image")
                     .Select(m => m.GetAttribute("src"))
                     .FirstOrDefault();

                var querySelector = new StringBuilder().Append("div > #textstart");

                while (!document.QuerySelector(querySelector.ToString()).OuterHtml.Contains("p"))
                {
                    querySelector.Append(" > div");
                }

                querySelector.Append(" p");

                var textParagraphs = document
                    .QuerySelectorAll(querySelector.ToString());

                var last = textParagraphs.Last();

                var text = new StringBuilder();

                textParagraphs
                    .Where(x => x.TextContent != "* * *")
                    .ToList()
                    .ForEach(x => text.AppendLine(x.TextContent.Replace("\n", "").Replace("↑", "")));

                //Replaces [xxxx]
                var notesRegex = new Regex(@"\[[\s\S]+?\]|\*\d+");
                var bookTextWithoutNotes = notesRegex.Replace(text.ToString(), string.Empty);

                text.Clear().AppendLine(bookTextWithoutNotes);

                //Console.WriteLine(text.ToString().TrimEnd());

                if (author != null)
                {
                    Console.WriteLine("Title " + title.TextContent.Trim());
                    Console.WriteLine("Category " + category.TextContent.Trim());
                    Console.WriteLine("Author " + author.TextContent.Trim());
                    Console.WriteLine("Year " + year.TextContent == null ? "n/a" : year.TextContent.Trim());
                    Console.WriteLine("Url " + imgUrl);
                    Console.WriteLine(new string('=', 50));
                }
                else
                {
                    Console.WriteLine("Null Author");
                }
            }
        }

    }
}
