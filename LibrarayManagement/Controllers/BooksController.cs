using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using LibraryManagement.Filters;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _repository;

        public BooksController(IBookRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetBooks()
        {
            var books = _repository.GetAllBooks();

            return Ok(books);
        }


        [HttpGet("{id}")]
        public IActionResult GetBookById(int id)
        {
            var book = _repository.GetBookById(id);

            if (book == null)
            {
                return NotFound("Book not found");
            }

            return Ok(book);
        }

        [HttpPost]
        [ServiceFilter(typeof(BookValidationFilter))]
        public IActionResult AddBook(Book book)
        {
            _repository.AddBook(book);

            return Ok("Book added successfully");
        }
    }
}