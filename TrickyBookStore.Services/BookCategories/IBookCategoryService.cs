using TrickyBookStore.Models;

namespace TrickyBookStore.Services.Books
{
    public interface IBookCategoryServive
    {
        BookCategory GetBookCategoryById(int categoryId);
    }
}
