﻿using AngleSharp;
using AngleSharp.Dom;
using Bookalytics.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bookalytics.Services
{
    public class BookScrapperService : IBookScrapperService
    {
        private const string BaseUrl = "https://chitanka.info/text/{0}/0";
        private const string RemoveNotesPattern = @"\[[\s\S]+?\]|\*\d+";
        private const string QuerySelectorAuthor = ".bookauthor";
        private const string QuerySelectorCategory = ".bookcat";

        private readonly IBrowsingContext context;

        public BookScrapperService()
        {
            var config = Configuration.Default.WithDefaultLoader();
            this.context = BrowsingContext.New(config);
        }

        public async Task<IDocument> GetDocumentAsync(int i)
        {
            var url = string.Format(BaseUrl, i);

            return await context.OpenAsync(url);
        }

        public IElement GetAuthor(IDocument document)
        {
            var author = document
                    .QuerySelectorAll(QuerySelectorAuthor)
                    .FirstOrDefault();

            return author;
        }

        public IElement GetCategory(IDocument document)
        {
            var category = document
                    .QuerySelectorAll(QuerySelectorCategory)
                    .FirstOrDefault();

            return category;
        }

        public IElement GetYear(IDocument document)
        {
            var year = document
                    .QuerySelectorAll("span")
                    .Where(x => x.GetAttribute("itemProp") == "datePublished")
                    .FirstOrDefault();

            return year;
        }

        public IElement GetTitle(IDocument document)
        {
            var title = document.QuerySelectorAll("i")
                   .Where(x => x.GetAttribute("itemProp") == "name")
                   .FirstOrDefault();

            return title;
        }

        public string GetImgUrl(IDocument document)
        {
            var imgUrl = document.QuerySelectorAll("img")
                     .Where(x => x.GetAttribute("itemProp") == "image")
                     .Select(m => m.GetAttribute("src"))
                     .FirstOrDefault();

            return imgUrl;
        }

        public string GetText(IDocument document)
        {
            var querySelector = new StringBuilder().Append("div > #textstart");

            while (!document.QuerySelector(querySelector.ToString()).OuterHtml.Contains("p"))
            {
                querySelector.Append(" > div");
            }

            querySelector.Append(" p");

            var paragraphs = document
                .QuerySelectorAll(querySelector.ToString());

            var text = FormatText(paragraphs);

            return text;
        }

        private string FormatText(IHtmlCollection<IElement> paragraphs)
        {
            var text = new StringBuilder();

            paragraphs
                .Where(x => x.TextContent != "* * *")
                .ToList()
                .ForEach(x => text.AppendLine(x.TextContent.Replace("\n", "").Replace("↑", "")));

            //Replaces [xxxx]
            var notesRegex = new Regex(RemoveNotesPattern);
            var bookTextWithoutNotes = notesRegex.Replace(text.ToString(), string.Empty);

            return bookTextWithoutNotes;
        }

        //public async Task Main()
        //{
        //    for (int i = 0; i < 1; i++)
        //    {
        //        var url = string.Format(BaseUrl,i);
        //        var document = await context.OpenAsync(url);


        //        if (author != null)
        //        {
        //            Console.WriteLine("Title " + title.TextContent.Trim());
        //            Console.WriteLine("Category " + category.TextContent.Trim());
        //            Console.WriteLine("Author " + author.TextContent.Trim());
        //            Console.WriteLine("Year " + year.TextContent == null ? "n/a" : year.TextContent.Trim());
        //            Console.WriteLine("Url " + imgUrl);
        //            Console.WriteLine(new string('=', 50));
        //        }
        //        else
        //        {
        //            Console.WriteLine("Null Author");
        //        }
        //    }
        //}

    }
}