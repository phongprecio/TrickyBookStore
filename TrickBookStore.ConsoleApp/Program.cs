using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrickyBookStore.Services.Books;
using TrickyBookStore.Services.Customers;
using TrickyBookStore.Services.Payment;
using TrickyBookStore.Services.PurchaseTransactions;
using TrickyBookStore.Services.Subscriptions;

namespace TrickBookStore.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IPaymentService, PaymentService>()
                .AddSingleton<ICustomerService, CustomerService>()
                .AddSingleton<IBookService, BookService>()
                .AddSingleton<IPurchaseTransactionService, PurchaseTransactionService>()
                .AddSingleton<IBookCategoryServive, BookCategoryService>()
                .AddSingleton<ISubscriptionService, SubscriptionService>()
                .BuildServiceProvider();

            Console.WriteLine("Date Format: YYYY-MM-DD");

            Console.WriteLine("Input Customer Id");
            var customerIdString = Console.ReadLine();
            var customerId = Convert.ToInt32(customerIdString);

            Console.WriteLine("Input From Date");
            var fromDateString = Console.ReadLine();
            var fromDate = DateTime.Parse(fromDateString);

            Console.WriteLine("Input To Date");
            var toDateString = Console.ReadLine();
            var toDate = DateTime.Parse(toDateString);

            var paymentService = serviceProvider.GetService<IPaymentService>();
            var paymentAmount = paymentService.GetPaymentAmount(customerId, fromDate, toDate);

            Console.WriteLine(paymentAmount);

            Console.WriteLine($"Press Any Key To Stop!");
            Console.ReadKey();
        }

        //private Tuple<int, DateTime, DateTime> GetParameter()
        //{
        //    Console.WriteLine("Date Format: YYYY-MM-DD");

        //    Console.WriteLine("Input Customer Id");
        //    var customerIdString = Console.ReadLine();
        //    var customerId = Convert.ToInt32(customerIdString);

        //    Console.WriteLine("Input Date From");
        //    var fromDateString = Console.ReadLine();
        //    var fromDate = DateTime.Parse(fromDateString);

        //    Console.WriteLine("Input Date To");
        //    var toDateString = Console.ReadLine();
        //    var toDate = DateTime.Parse(toDateString);

        //    return Tuple.Create(customerId, fromDate, toDate);
        //}
    }
}
