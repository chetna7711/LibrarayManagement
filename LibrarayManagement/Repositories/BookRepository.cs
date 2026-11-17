using LibraryManagement.Interfaces;
using LibraryManagement.Models;

namespace LibraryManagement.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly List<Book> _books = new()
        {
            new Book
            {
                Id = 1,
                Title = "C# Basics",
                Author = "John",
                Price = 500
            },

            new Book
            {
                Id = 2,
                Title = "ASP.NET Core",
                Author = "David",
                Price = 700
            },

            new Book
            {
                Id = 3,
                Title = "SQL Basics",
                Author = "Peter",
                Price = 400
            }
        };

        public List<Book> GetAllBooks()
        {
            return _books;
        }

        public Book GetBookById(int id)
        {
            return _books.FirstOrDefault(b => b.Id == id);
        }

        public void AddBook(Book book)
        {
            book.Id = _books.Max(b => b.Id) + 1;

            _books.Add(book);
        }
    }
}