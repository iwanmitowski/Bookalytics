using AutoMapper;
using Bookalytics.Data.Models;
using Bookalytics.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookalytics.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<BookInputModel, Book>();
            this.CreateMap<Book, BookViewModel>();
        }
    }
}
