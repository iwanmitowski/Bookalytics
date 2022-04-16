using AutoMapper;

using Bookalytics.Data.Models;
using Bookalytics.ViewModels;

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
