using System;
using System.Collections.Generic;
using System.Linq;
using TrickyBookStore.Models;
using TrickyBookStore.Services.Books;

namespace TrickyBookStore.Services.PurchaseTransactions
{
    public class PurchaseTransactionService : IPurchaseTransactionService
    {
        IBookService BookService { get; }

        public PurchaseTransactionService(IBookService bookService)
        {
            BookService = bookService;
        }

        public IList<PurchaseTransaction> GetPurchaseTransactions(long customerId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            var purchaseTransactions = Store.PurchaseTransactions.Data.Where(transaction => customerId == transaction.CustomerId && fromDate <= transaction.CreatedDate && toDate >= transaction.CreatedDate).ToList();
            purchaseTransactions.ForEach(transaction =>
            {
                transaction.Book = BookService.GetBookById(transaction.BookId);
            });

            return purchaseTransactions;
        }
    }
}
