using AngleSharp;
using AngleSharp.Dom;
using Bookalytics.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

        public string GetAuthor(IDocument document)
        {
            var author = document
                    .QuerySelectorAll(QuerySelectorAuthor)
                    .FirstOrDefault();

            return author?.TextContent;
        }

        public string GetCategory(IDocument document)
        {
            var category = document
                    .QuerySelectorAll(QuerySelectorCategory)
                    .FirstOrDefault();

            return category?.TextContent;
        }

        public int? GetYear(IDocument document)
        {
            var year = document
                    .QuerySelectorAll("span")
                    .Where(x => x.GetAttribute("itemProp") == "datePublished")
                    .FirstOrDefault();

            int yearAsInt;

            if (!int.TryParse(year?.TextContent, out yearAsInt))
            {
                return null;
            }

            return yearAsInt;
        }

        public string GetTitle(IDocument document)
        {
            var title = document.QuerySelectorAll("i")
                   .Where(x => x.GetAttribute("itemProp") == "name")
                   .FirstOrDefault();

            return title?.TextContent;
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
            //string outherHtml = document.QuerySelector(querySelector.ToString()) == null ? null : document.QuerySelector(querySelector.ToString()).OuterHtml;

            //if (outherHtml == null)
            //{
            //    return null;
            //}

            //while (!outherHtml.Contains("p"))
            //{
            //    querySelector.Append(" > div");
            //    outherHtml = document.QuerySelector(querySelector.ToString()).OuterHtml;

            //    if (outherHtml == null)
            //    {
            //        return null;
            //    }
            //}

            while (true)
            {
                string outherHtml = document.QuerySelector(querySelector.ToString()) == null ? null : document.QuerySelector(querySelector.ToString()).OuterHtml;

                if (outherHtml == null)
                {
                    return null;
                }
                else if (outherHtml.Contains("p"))
                {
                    break;
                }

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
    }
}
