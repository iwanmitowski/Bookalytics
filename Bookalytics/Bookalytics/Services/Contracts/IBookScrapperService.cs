using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;

namespace Bookalytics.Services.Contracts
{
    public interface IBookScrapperService
    {
        public Task<IDocument> GetDocumentAsync(int i);
        public string GetAuthor(IDocument document);
        public string GetCategory(IDocument document);
        public int? GetYear(IDocument document);
        public string GetTitle(IDocument document);
        public string GetImgUrl(IDocument document);
        public string GetText(IDocument document);
    }
}
