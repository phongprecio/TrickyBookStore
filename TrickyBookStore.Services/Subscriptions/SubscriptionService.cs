using System;
using System.Collections.Generic;
using System.Linq;
using TrickyBookStore.Models;

namespace TrickyBookStore.Services.Subscriptions
{
    public class SubscriptionService : ISubscriptionService
    {
        public IList<Subscription> GetSubscriptions(params int[] ids)
        {
            return Store.Subscriptions.Data.OrderBy(subscription => subscription.Priority).Where(subscription => ids.Contains(subscription.Id)).ToList();
        }
    }
}
