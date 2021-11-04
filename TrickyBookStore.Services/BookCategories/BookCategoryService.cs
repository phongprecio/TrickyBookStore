using System.Linq;
using TrickyBookStore.Models;

namespace TrickyBookStore.Services.Books
{
    public class BookCategoryService : IBookCategoryServive
    {
        public BookCategory GetBookCategoryById(int id)
        {
            return Store.BookCategories.Data.FirstOrDefault(bookCategory => id == bookCategory.Id);
        }
    }
}
