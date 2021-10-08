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

            //Start from here
            seeder.GetBooks(dbContext, serviceProvider);
            //await seeder.FillBooksData(dbContext, serviceProvider);

            await seeder.SeedAsync(dbContext, serviceProvider);
            await dbContext.SaveChangesAsync();
        }
    }
}
