using Bookalytics.Data;
using Bookalytics.Seeding.Contracts;
using Bookalytics.Services.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Bookalytics.Seeding
{
    public class ApplicationDbContextSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var seeder = new BooksSeeder();
            var bookAnalyzer = serviceProvider.GetService<IBookAnalyzerService>();

            //Start from here
            await seeder.GetBooks(dbContext, serviceProvider);
            seeder.FillBooksData(bookAnalyzer);

            await seeder.SeedAsync(dbContext, serviceProvider);
            await dbContext.SaveChangesAsync();
        }
    }
}
