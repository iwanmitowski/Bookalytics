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
        public IElement GetAuthor(IDocument document);
        public IElement GetCategory(IDocument document);
        public IElement GetYear(IDocument document);
        public IElement GetTitle(IDocument document);
        public string GetImgUrl(IDocument document);
        public string GetText(IDocument document);
    }
}
