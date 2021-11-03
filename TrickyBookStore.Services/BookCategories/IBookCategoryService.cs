using TrickyBookStore.Models;

namespace TrickyBookStore.Services.Books
{
    public interface IBookCategoryServive
    {
        BookCategory GetBookCategory(int id);
    }
}
