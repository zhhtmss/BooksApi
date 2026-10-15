using BooksApi.Controllers;
using BooksApi.Models;
using BooksApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BooksApi.Tests
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookRepository> _mockRepository;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _mockRepository = new Mock<IBookRepository>();
            _controller = new BooksController(_mockRepository.Object);
        }

        // 1. GET: повертає всі книги
        [Fact]
        public void GetAll_ReturnsAllBooks()
        {
            var books = new List<Book>
        {
            new Book { Id = 1, Title = "Кобзар", Author = "Тарас Шевченко", Year = 1840 },
            new Book { Id = 2, Title = "Лісова пісня", Author = "Леся Українка", Year = 1911 }
        };

            _mockRepository.Setup(r => r.GetAll()).Returns(books);

            var result = _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBooks = Assert.IsAssignableFrom<IEnumerable<Book>>(
                okResult.Value);

            Assert.Equal(2, returnedBooks.Count());
        }

        // 2. GET: повертає порожній список
        [Fact]
        public void GetAll_ReturnsEmptyList()
        {
            _mockRepository.Setup(r => r.GetAll())
                .Returns(new List<Book>());

            var result = _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var books = Assert.IsAssignableFrom<IEnumerable<Book>>(
                okResult.Value);

            Assert.Empty(books);
        }

        // 3. GET by ID: книга існує
        [Fact]
        public void GetById_ReturnsBook_WhenBookExists()
        {
            var book = new Book
            {
                Id = 1,
                Title = "Кобзар",
                Author = "Тарас Шевченко",
                Year = 1840
            };

            _mockRepository.Setup(r => r.GetById(1)).Returns(book);

            var result = _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBook = Assert.IsType<Book>(okResult.Value);

            Assert.Equal("Кобзар", returnedBook.Title);
            Assert.Equal(1, returnedBook.Id);
        }

        // 4. GET by ID: книга не знайдена
        [Fact]
        public void GetById_ReturnsNotFound_WhenBookDoesNotExist()
        {
            _mockRepository.Setup(r => r.GetById(99)).Returns((Book?)null);

            var result = _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // 5. POST: успішне додавання книги
        [Fact]
        public void Create_ReturnsCreated_WhenBookIsValid()
        {
            var book = new Book
            {
                Id = 1,
                Title = "Кобзар",
                Author = "Тарас Шевченко",
                Year = 1840
            };

            var result = _controller.Create(book);

            var createdResult = Assert.IsType<CreatedAtActionResult>(
                result.Result);

            Assert.Equal(nameof(BooksController.GetById),
                createdResult.ActionName);

            _mockRepository.Verify(
                r => r.Add(It.Is<Book>(b => b.Title == "Кобзар")),
                Times.Once);
        }

        // 6. POST: порожня назва книги
        [Fact]
        public void Create_ReturnsBadRequest_WhenTitleIsEmpty()
        {
            var book = new Book
            {
                Title = "",
                Author = "Невідомий автор",
                Year = 2020
            };

            var result = _controller.Create(book);

            Assert.IsType<BadRequestObjectResult>(result.Result);

            _mockRepository.Verify(
                r => r.Add(It.IsAny<Book>()),
                Times.Never);
        }

        // 7. PUT: успішне оновлення книги
        [Fact]
        public void Update_ReturnsNoContent_WhenBookExists()
        {
            var existingBook = new Book
            {
                Id = 1,
                Title = "Стара назва",
                Author = "Автор",
                Year = 2020
            };

            var updatedBook = new Book
            {
                Title = "Нова назва",
                Author = "Інший автор",
                Year = 2025
            };

            _mockRepository.Setup(r => r.GetById(1))
                .Returns(existingBook);

            var result = _controller.Update(1, updatedBook);

            Assert.IsType<NoContentResult>(result);

            _mockRepository.Verify(
                r => r.Update(It.Is<Book>(b =>
                    b.Id == 1 && b.Title == "Нова назва")),
                Times.Once);
        }

        // 8. PUT: книга не знайдена
        [Fact]
        public void Update_ReturnsNotFound_WhenBookDoesNotExist()
        {
            _mockRepository.Setup(r => r.GetById(99))
                .Returns((Book?)null);

            var book = new Book
            {
                Title = "Нова книга",
                Author = "Автор",
                Year = 2025
            };

            var result = _controller.Update(99, book);

            Assert.IsType<NotFoundResult>(result);

            _mockRepository.Verify(
                r => r.Update(It.IsAny<Book>()),
                Times.Never);
        }

        // 9. DELETE: успішне видалення
        [Fact]
        public void Delete_ReturnsNoContent_WhenBookExists()
        {
            var book = new Book
            {
                Id = 1,
                Title = "Кобзар",
                Author = "Тарас Шевченко",
                Year = 1840
            };

            _mockRepository.Setup(r => r.GetById(1))
                .Returns(book);

            var result = _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);

            _mockRepository.Verify(
                r => r.Delete(1),
                Times.Once);
        }

        // 10. DELETE: книга не знайдена
        [Fact]
        public void Delete_ReturnsNotFound_WhenBookDoesNotExist()
        {
            _mockRepository.Setup(r => r.GetById(99))
                .Returns((Book?)null);

            var result = _controller.Delete(99);

            Assert.IsType<NotFoundResult>(result);

            _mockRepository.Verify(
                r => r.Delete(It.IsAny<int>()),
                Times.Never);
        }
    }
}
