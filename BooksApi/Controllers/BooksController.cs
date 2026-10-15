using BooksApi.Models;
using BooksApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BooksApi.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _repository;

        public BooksController(IBookRepository repository)
        {
            _repository = repository;
        }

        // GET /api/books
        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetAll()
        {
            return Ok(_repository.GetAll());
        }

        // GET /api/books/{id}
        [HttpGet("{id}")]
        public ActionResult<Book> GetById(int id)
        {
            var book = _repository.GetById(id);

            if (book == null)
                return NotFound();

            return Ok(book);
        }

        // POST /api/books
        [HttpPost]
        public ActionResult<Book> Create(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.Title))
                return BadRequest("Назва книги не може бути порожньою.");

            _repository.Add(book);

            return CreatedAtAction(
                nameof(GetById),
                new { id = book.Id },
                book
            );
        }

        // PUT /api/books/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, Book book)
        {
            var existingBook = _repository.GetById(id);

            if (existingBook == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(book.Title))
                return BadRequest("Назва книги не може бути порожньою.");

            book.Id = id;

            _repository.Update(book);

            return NoContent();
        }

        // DELETE /api/books/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var book = _repository.GetById(id);

            if (book == null)
                return NotFound();

            _repository.Delete(id);

            return NoContent();
        }
    }
}
