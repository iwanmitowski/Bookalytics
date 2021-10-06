using Bookalytics.Data;
using Bookalytics.Seeding.Contracts;
using Bookalytics.Services;
using Bookalytics.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Bookalytics.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using AutoMapper;
using Bookalytics.Data.Models;
using System.Collections.Concurrent;

namespace Bookalytics.Seeding
{
    public class BooksSeeder : ISeeder, IBookPreparer
    {
        private int BaseSeedingCount = 2500;
        private const string BaseUrl = "https://chitanka.info/text/{0}/0";

        private readonly ConcurrentBag<BookInputModel> bookInputs;

        public BooksSeeder()
        {
            this.bookInputs = new ConcurrentBag<BookInputModel>();
        }

        public async Task GetBooks(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Books.Any())
            {
                return;
            }

            var scrapperService = serviceProvider.GetService<IBookScrapperService>();

            for (int i = 1; i <= 5; i++)
            {
                var url = string.Format(BaseUrl, i);

                var document = await scrapperService.GetDocumentAsync(i);

                var author = scrapperService.GetAuthor(document);

                var text = scrapperService.GetText(document);

                if (author == null || text == null)
                {
                    continue;
                    //return;  //Parallel for is like function
                }

                var year = scrapperService.GetYear(document);
                var imgUrl = scrapperService.GetImgUrl(document);

                var currentBook = new BookInputModel();
                currentBook.Author = author;
                currentBook.Text = text;

                bookInputs.Add(currentBook);
            }
            //Parallel.For(1, BaseSeedingCount, async i =>
            //{
                
            //    i++;
            //});
        }

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Books.Any())
            {
                return;
            }
            var mapper = serviceProvider.GetService<IMapper>();

            var books = mapper.Map<IEnumerable<Book>>(bookInputs);

            await dbContext.AddRangeAsync(books);
            await dbContext.SaveChangesAsync();
        }

        public void FillBooksData(IBookAnalyzerService bookAnalyzer)
        {
            //Must be tested!!!

            //Parallel.ForEach(bookInputs, (book) =>
            //{
            //    bookAnalyzer.GetText(book.Text);
            //    book.WordsCount = bookAnalyzer.GetWordsCount();
            //    book.ShortestWord = bookAnalyzer.GetShortestWord();
            //    book.LongestWord = bookAnalyzer.GetLongestWord();
            //    book.MostCommonWord = bookAnalyzer.GetMostCommonWord();
            //    book.MostCommonWordCount = bookAnalyzer.GetMostCommonWordCount(book.MostCommonWord);
            //    book.LeastCommonWord = bookAnalyzer.GetLeastCommonWord();
            //    book.LeastCommonWordCount = bookAnalyzer.GetLeastCommonWordCount(book.LeastCommonWord);
            //    book.AverageWordLength = bookAnalyzer.GetAverageWordLength();
            //});

            foreach (var book in bookInputs)
            {
                bookAnalyzer.GetText(book.Text);
                book.WordsCount = bookAnalyzer.GetWordsCount();
                book.ShortestWord = bookAnalyzer.GetShortestWord();
                book.LongestWord = bookAnalyzer.GetLongestWord();
                book.MostCommonWord = bookAnalyzer.GetMostCommonWord();
                book.MostCommonWordCount = bookAnalyzer.GetMostCommonWordCount(book.MostCommonWord);
                book.LeastCommonWord = bookAnalyzer.GetLeastCommonWord();
                book.LeastCommonWordCount = bookAnalyzer.GetLeastCommonWordCount(book.LeastCommonWord);
                book.AverageWordLength = bookAnalyzer.GetAverageWordLength();
            }
        }
    }
}
