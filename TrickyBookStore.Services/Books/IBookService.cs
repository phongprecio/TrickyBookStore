using System.Collections.Generic;
using TrickyBookStore.Models;

namespace TrickyBookStore.Services.Books
{
    public interface IBookService
    {
        IList<Book> GetBooks(params long[] ids);
        Book GetBookById(long id);
    }
}
