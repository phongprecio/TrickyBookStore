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
            var categoryDic = new Dictionary<int, BookCategory>();
            var categoriyIds = books.Select(book => book.CategoryId).Distinct().ToList();
            
            categoriyIds.ForEach(categoryId =>
            {
                var bookCategory = BookCategoryServicve.GetBookCategoryById(categoryId);
                if (bookCategory != null)
                    categoryDic.Add(categoryId, bookCategory);
            });

            books.ForEach(book => book.Category = categoryDic[book.CategoryId]);
            
            return books;
        }
    }
}
