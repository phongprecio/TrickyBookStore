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

        private static int LimitedDiscountBook = 3;

        public PaymentService(ICustomerService customerService, 
            IPurchaseTransactionService purchaseTransactionService,
            IBookService bookService)
        {
            CustomerService = customerService;
            PurchaseTransactionService = purchaseTransactionService;
            BookService = bookService;
        }

        public double GetPaymentAmount(PaymentAmountSearchParameter searchParameter)
        {
            var purcharseTransactions = PurchaseTransactionService.GetPurchaseTransactions(searchParameter);

            return PaymentAmount(purcharseTransactions, searchParameter);
        }

        private double PaymentAmount(IList<PurchaseTransaction> purcharseTransactions, PaymentAmountSearchParameter searchParameter)
        {
            double amount = 0;
            var categoryAddictedIds = new List<int>();
            var customer = CustomerService.GetCustomerById(searchParameter.CustomerId);

            var totalMonth = searchParameter.ToDate.Month - searchParameter.FromDate.Month + 1;
            amount += CalculatorSubscriptionOfMonth(customer, amount) * totalMonth;

            var purcharseTransactionDics = purcharseTransactions
                                                .GroupBy(transaction => transaction.CreatedDate.DateTime.Month)
                                                .ToDictionary(transaction => transaction.Key, transaction => transaction.ToList());

            if (customer.SubscriptionIds.Any())
                categoryAddictedIds = customer.Subscriptions
                                            .Where(subscription => subscription.SubscriptionType == SubscriptionTypes.CategoryAddicted)
                                            .Select(subscription => subscription.Id).ToList();

            foreach (var purcharseTransactionDic in purcharseTransactionDics)
            {
                if (purcharseTransactions.Any())
                {
                    var books = purcharseTransactionDic.Value.Select(transaction => transaction.Book).ToList();
                    var subscriptionTypes = customer.Subscriptions.Select(subscription => subscription.SubscriptionType).Distinct().ToList();

                    if (customer.SubscriptionIds.Any())
                    {
                        if (subscriptionTypes.Contains(SubscriptionTypes.CategoryAddicted))
                            amount = CalculatorCategoryAddictedPrice(ref books, amount, categoryAddictedIds);
                        if (subscriptionTypes.Contains(SubscriptionTypes.Premium))
                            amount = CalculatorPremiumPrice(ref books, amount);
                        if (subscriptionTypes.Contains(SubscriptionTypes.Paid))
                            amount = CalculatorPaidPrice(ref books, amount);
                        if (subscriptionTypes.Contains(SubscriptionTypes.Free))
                            amount = CalculatorFreePrice(ref books, amount);
                    }

                    books.ForEach(book => amount += book.Price);
                }
            }


            return amount;
        }

        private double CalculatorSubscriptionOfMonth(Customer customer, double amount)
        {
            customer.Subscriptions.ToList().ForEach(subscription => amount += subscription.PriceDetails["FixPrice"]);
            return amount;
        }

        private double CalculatorPremiumPrice(ref List<Book> books, double amount)
        {
            var oldBookIds = books.Where(book => book.IsOld == true).Select(book => book.Id).ToList();
            var countedBookIds = new List<long>();

            for (var i = 0; i < LimitedDiscountBook && i < books.Count; i++)
            {
                amount += books[i].Price * 0.85;
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
                for (var i = 0; i < LimitedDiscountBook && i < discountBookByCategories.Count; i++)
                {
                    amount += discountBookByCategories[i].Price * 0.85;
                    countedBookIds.Add(discountBookByCategories[i].Id);
                }
            });
            books = books.FindAll(book => !(countedBookIds.Contains(book.Id) || oldBookIds.Contains(book.Id)));

            return amount;
        }

        private double CalculatorPaidPrice(ref List<Book> books, double amount)
        {
            var oldBooks = books.Where(book => book.IsOld == true).ToList();
            var discountBooks = books.Where(book => book.IsOld == false).ToList();
            var countedBookIds = new List<long>();

            oldBooks.ForEach(book =>
            {
                amount += book.Price * 0.05;
                countedBookIds.Add(book.Id);
            });

            for (var i = 0; i < LimitedDiscountBook && i < discountBooks.Count; i++)
            {
                amount += discountBooks[i].Price * 0.95;
                countedBookIds.Add(discountBooks[i].Id);
            }
            books = books.FindAll(book => !countedBookIds.Contains(book.Id));

            return amount;
        }

        private double CalculatorFreePrice(ref List<Book> books, double amount)
        {
            var oldBooks = books.FindAll(book => book.IsOld == true).ToList();
            var newBooks = books.FindAll(book => book.IsOld == false).ToList();

            oldBooks.ForEach(book => amount += book.Price * 0.9);
            newBooks.ForEach(book => amount += book.Price);

            books = new List<Book>();

            return amount;
        }
    }
}
