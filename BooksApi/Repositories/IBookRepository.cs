using BooksApi.Models;

namespace BooksApi.Repositories
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetAll();

        Book? GetById(int id);

        void Add(Book book);

        void Update(Book book);

        void Delete(int id);
    }
}
