using Bookalytics.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookalytics.Seeding.Contracts
{
    public interface ISeeder
    {
        public Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider);
    }
}
