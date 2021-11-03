using System;
using System.Collections.Generic;
using System.Linq;
using TrickyBookStore.Models;
using TrickyBookStore.Services.Books;
using TrickyBookStore.Services.Customers;
using TrickyBookStore.Services.PurchaseTransactions;

namespace TrickyBookStore.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        ICustomerService CustomerService { get; }
        IBookService BookService { get; }
        IPurchaseTransactionService PurchaseTransactionService { get; }

        public PaymentService(ICustomerService customerService, 
            IPurchaseTransactionService purchaseTransactionService,
            IBookService bookService)
        {
            CustomerService = customerService;
            PurchaseTransactionService = purchaseTransactionService;
            BookService = bookService;
        }

        public double GetPaymentAmount(long customerId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            var customer = CustomerService.GetCustomerById(customerId);
            var purcharseTransactions = PurchaseTransactionService.GetPurchaseTransactions(customer.Id, fromDate, toDate);

            return PaymentAmount(purcharseTransactions, customer);
        }

        private double PaymentAmount(IList<PurchaseTransaction> purcharseTransactions, Customer customer)
        {
            double amount = 0;
            var categoryAddictedIds = new List<int>();
            if (customer.SubscriptionIds.Any())
            {
                customer.Subscriptions.ToList().ForEach(subscription => amount += subscription.PriceDetails["FixPrice"]);
                categoryAddictedIds = customer.Subscriptions
                                        .Where(subscription => subscription.SubscriptionType == SubscriptionTypes.CategoryAddicted)
                                        .Select(subscription => subscription.Id).ToList();
            }

            if (purcharseTransactions.Any())
            {
                var books = purcharseTransactions.Select(transaction => transaction.Book).ToList();
                var subscriptionTypes = customer.Subscriptions.Select(subscription => subscription.SubscriptionType).Distinct().ToList();

                if (customer.SubscriptionIds.Any())
                {
                    if (subscriptionTypes.Contains(SubscriptionTypes.CategoryAddicted))
                        amount += CalculatorCategoryAddictedPrice(ref books, amount, categoryAddictedIds);
                    if (subscriptionTypes.Contains(SubscriptionTypes.Premium))
                        amount += CalculatorPremiumPrice(ref books, amount, categoryAddictedIds);
                    if (subscriptionTypes.Contains(SubscriptionTypes.Paid))
                        amount += CalculatorPaidPrice(ref books, amount);
                    if (subscriptionTypes.Contains(SubscriptionTypes.Free))
                        amount += CalculatorFreePrice(books, amount);
                }

                books.ForEach(book => amount += book.Price);
            }

            return amount;
        }

        private double CalculatorPremiumPrice(ref List<Book> books, double amount, List<int> categoryAddictedIds)
        {
            var oldBookIds = books.Where(book => book.IsOld == true).Select(book => book.Id).ToList();
            var countedBookIds = new List<long>();

            for (var i = 0; i < 3 && i < books.Count; i++)
            {
                amount += books[i].Price * 0.15;
                countedBookIds.Add(books[i].Id);
            }

            books = books.FindAll(book => !(countedBookIds.Contains(book.Id) || oldBookIds.Contains(book.Id)));

            return amount;
        }

        private double CalculatorCategoryAddictedPrice(ref List<Book> books, double amount, List<int> categoryAddictedIds)
        {
            var bookGroups = books.GroupBy(book => book.CategoryId);
            var discountBooks = books.Where(book => categoryAddictedIds.Contains(book.CategoryId) && book.IsOld == false).ToList();
            var oldBookIds = books.Where(book => categoryAddictedIds.Contains(book.CategoryId) && book.IsOld == true).Select(book => book.Id).ToList();
            var countedBookIds = new List<long>();

            categoryAddictedIds.ForEach(categoryId =>
            {
                var discountBookByCategories = discountBooks.Where(book => book.CategoryId == categoryId).ToList();
                for (var i = 0; i < 3 && i < discountBookByCategories.Count; i++)
                {
                    amount += discountBookByCategories[i].Price * 0.15;
                    countedBookIds.Add(discountBookByCategories[i].Id);
                }
            });
            books = books.FindAll(book => !(countedBookIds.Contains(book.Id) || oldBookIds.Contains(book.Id)));

            return amount;
        }

        private double CalculatorPaidPrice(ref List<Book> books, double amount)
        {
            var oldBooks = books.Where(book => book.IsOld == true).ToList();
            var countedBookIds = new List<long>();

            oldBooks.ForEach(book =>
            {
                amount += book.Price * 0.05;
                countedBookIds.Add(book.Id);
            });

            for (var i = 0; i < 3 && i < books.Count; i++)
            {
                amount += books[i].Price * 0.95;
                countedBookIds.Add(books[i].Id);
            }
            books = books.FindAll(book => !countedBookIds.Contains(book.Id));

            return amount;
        }

        private double CalculatorFreePrice(List<Book> books, double amount)
        {
            var oldBooks = books.FindAll(book => book.IsOld == true).ToList();
            var newBooks = books.FindAll(book => book.IsOld == false).ToList();

            oldBooks.ForEach(book => amount += book.Price * 0.9);
            newBooks.ForEach(book => amount += book.Price);

            return amount;
        }
    }
}
