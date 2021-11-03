using System;
using System.Linq;
using TrickyBookStore.Models;
using TrickyBookStore.Services.Subscriptions;

namespace TrickyBookStore.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        ISubscriptionService SubscriptionService { get; }

        public CustomerService(ISubscriptionService subscriptionService)
        {
            SubscriptionService = subscriptionService;
        }

        public Customer GetCustomerById(long id)
        {
            var customer = Store.Customers.Data.FirstOrDefault(item => item.Id == id);
            customer.Subscriptions = SubscriptionService.GetSubscriptions(customer.SubscriptionIds.ToArray());

            return customer;
        }
    }
}
