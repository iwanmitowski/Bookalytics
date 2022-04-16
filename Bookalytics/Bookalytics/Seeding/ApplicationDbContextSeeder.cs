using System;
using System.Threading.Tasks;

using Bookalytics.Data;
using Bookalytics.Seeding.Contracts;

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
        }
    }
}
