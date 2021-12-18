using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Bookalytics.Data;
using Bookalytics.Data.Models;
using Bookalytics.Services.Contracts;
using Bookalytics.ViewModels;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Bookalytics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IBookAnalyzerService bookAnalyzerService;

        public BooksController(ApplicationDbContext dbContext, IMapper mapper, IBookAnalyzerService bookAnalyzerService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.bookAnalyzerService = bookAnalyzerService;
        }

        //GET api/books
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookViewModel))]
        public IActionResult GetAll()
        {
            var books = mapper.Map<IEnumerable<BookViewModel>>(dbContext.Books.Take(10).ToList());

            foreach (var book in books)
            {
                book.Text = string.Empty;
            }

            return Ok(books);
        }

        //GET api/books/id
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookViewModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var book = mapper.Map<BookViewModel>(await dbContext.Books.FirstOrDefaultAsync(x => x.Id == id));

            if (book == null)
            {
                return NotFound();
            }

            book.Text = string.Empty;

            return Ok(book);
        }

        //POST api/books
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookViewModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddBook(AddBookInputModel input)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}

            var book = new Book();

            book.Text = input.Text;

            //await dbContext.Books.AddAsync(book);
            //await dbContext.SaveChangesAsync();

            var response = mapper.Map<BookViewModel>(book);

            return Ok(response);
        }

    }
}
