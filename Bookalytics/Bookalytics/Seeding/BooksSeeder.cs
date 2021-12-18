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
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using AngleSharp.Dom;
using System.Threading;

namespace Bookalytics.Seeding
{
    public class BooksSeeder : ISeeder, IBookPreparer
    {
        private const int BaseSeedingCount = 150;

        private readonly ConcurrentBag<BookInputModel> bookInputs;

        public BooksSeeder()
        {
            this.bookInputs = new ConcurrentBag<BookInputModel>();
        }

        public void GetBooks(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Books.Any())
            {
                return;
            }

            var scrapperService = serviceProvider.GetService<IBookScrapperService>();

            var listOfThreads = new List<Thread>()
            {
                new Thread(() => GenerateBooks(scrapperService, 1, BaseSeedingCount)),
                new Thread(() => GenerateBooks(scrapperService, 4 * BaseSeedingCount, 5 * BaseSeedingCount)),
            };

            foreach (var thread in listOfThreads)
            {
                thread.Start();
            }

            foreach (var thread in listOfThreads)
            {
                thread.Join();
            }
        }

        private void GenerateBooks(IBookScrapperService scrapperService, int start, int end)
        {
            Parallel.For(start, end + 1, i =>
            {
                lock (new object())
                {
                    try
                    {
                        var currentBook = new BookInputModel();
                        IDocument document;

                        document = scrapperService.GetDocumentAsync(i).GetAwaiter().GetResult();

                        var author = scrapperService.GetAuthor(document).Replace("\n\t\t\t", " ").Trim();
                        var title = scrapperService.GetTitle(document);
                        var text = scrapperService.GetText(document);

                        if (author == null || text == null)
                        {
                            return;
                        }

                        var year = scrapperService.GetYear(document);
                        var imgUrl = scrapperService.GetImgUrl(document);

                        currentBook.Author = author;
                        currentBook.Title = title;
                        currentBook.Text = text;
                        currentBook.Year = year;
                        currentBook.ImageUrl = imgUrl;

                        bookInputs.Add(currentBook);
                    }
                    catch (NullReferenceException)
                    {
                        //ignored
                    }
                }
            });
        }

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Books.Any())
            {
                return;
            }

            var mapper = serviceProvider.GetService<IMapper>();

            var books = mapper.Map<ICollection<Book>>(bookInputs);

            FillBooksData(books, serviceProvider);

            //Adding one by one because it caused db problems
            foreach (var book in books)
            {
                await dbContext.Books.AddAsync(book);
                await dbContext.SaveChangesAsync();
            }
        }

        public void FillBooksData(ICollection<Book> books, IServiceProvider serviceProvider)
        {
            var bookAnalyzer = serviceProvider.GetService<IBookAnalyzerService>();

            foreach (var book in books)
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
