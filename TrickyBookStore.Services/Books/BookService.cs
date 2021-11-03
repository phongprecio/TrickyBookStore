using System;
using System.Collections.Generic;
using System.Linq;
using TrickyBookStore.Models;

namespace TrickyBookStore.Services.Books
{
    public class BookService : IBookService
    {
        IBookCategoryServive BookCategoryServicve;

        public BookService(IBookCategoryServive bookCategoryServicve)
        {
            BookCategoryServicve = bookCategoryServicve;
        }

        public Book GetBookById(long id)
        {
            return Store.Books.Data.FirstOrDefault(book => id == book.Id);
        }

        public IList<Book> GetBooks(params long[] ids)
        {
            var books = Store.Books.Data.Where(book => ids.Contains(book.Id)).ToList();
            books.ForEach(book =>
            {
                book.Category = BookCategoryServicve.GetBookCategory(book.CategoryId);
            });
            
            return books;
        }

        public IList<Book> GetBooksByCategoryId(int categoryId)
        {
            return Store.Books.Data.Where(book => categoryId == book.CategoryId).ToList();
        }
    }
}
