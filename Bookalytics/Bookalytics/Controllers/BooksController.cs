using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Bookalytics.Data;
using Bookalytics.Services.Contracts;
using Bookalytics.ViewModels;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookalytics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken] //With AntiforgeryToken the server returns 400 BadRequest
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
        public async Task<IActionResult> AddBook(AddBookInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            var book = bookAnalyzerService.Analyze(input);

            await dbContext.Books.AddAsync(book);
            await dbContext.SaveChangesAsync();

            var response = mapper.Map<BookViewModel>(book);

            return Ok(response);
        }

        //PUT api/books/{id} 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, UpdateBookInputModel inputModel)
        {
            var originalBook = await dbContext.Books.FindAsync(id);

            if (!ModelState.IsValid || originalBook == null)
            {
                return BadRequest();
            }

            originalBook.Title = inputModel.Title;
            originalBook.Author = inputModel.Author;
            originalBook.Year = inputModel.Year;
            originalBook.ImageUrl = inputModel.ImageUrl;

            await dbContext.SaveChangesAsync();

            return Ok(originalBook);
        }
    }
}
