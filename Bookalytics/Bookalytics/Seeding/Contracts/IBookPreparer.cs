﻿using Bookalytics.Data;
using Bookalytics.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookalytics.Seeding.Contracts
{
    public interface IBookPreparer
    {
        public void GetBooks(ApplicationDbContext dbContext, IServiceProvider serviceProvider);
        public void FillBooksData(IServiceProvider serviceProvider);
    }
}
