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

        public IList<PurchaseTransaction> GetPurchaseTransactions(PaymentAmountSearchParameter searchParameter)
        {
            var purchaseTransactions = Store.PurchaseTransactions.Data.Where(transaction => searchParameter.CustomerId == transaction.CustomerId && searchParameter.FromDate <= transaction.CreatedDate && searchParameter.ToDate >= transaction.CreatedDate).ToList();
            purchaseTransactions.ForEach(transaction =>
            {
                transaction.Book = BookService.GetBookById(transaction.BookId);
            });

            return purchaseTransactions;
        }
    }
}
