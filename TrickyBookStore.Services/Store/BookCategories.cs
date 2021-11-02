using System.Collections.Generic;
using TrickyBookStore.Models;

namespace TrickyBookStore.Services.Store
{
    public static class BookCategories
    {
        public static readonly IEnumerable<BookCategory> Data = new List<BookCategory>
        {
            new BookCategory { Id = 1, Title = "Science" },
            new BookCategory { Id = 2, Title = "Action and Adventure" },
            new BookCategory { Id = 3, Title = "Detective and Mystery" },
            new BookCategory { Id = 4, Title = "Historical Fiction" }
        };
    }
}
