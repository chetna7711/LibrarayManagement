using LibraryManagement.Models;

namespace LibraryManagement.Interfaces
{
    public interface IBookRepository
    {
        List<Book> GetAllBooks();

        Book GetBookById(int id);

        void AddBook(Book book);
    }
}