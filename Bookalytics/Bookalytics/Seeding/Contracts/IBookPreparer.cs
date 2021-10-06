using Bookalytics.Data;
using Bookalytics.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookalytics.Seeding.Contracts
{
    public interface IBookPreparer
    {
        public Task GetBooks(ApplicationDbContext dbContext, IServiceProvider serviceProvider);
        public void FillBooksData(IBookAnalyzerService bookAnalyzerService);
    }
}
