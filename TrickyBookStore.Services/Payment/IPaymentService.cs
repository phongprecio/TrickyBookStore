using System;
using TrickyBookStore.Models;

// KeepIt
namespace TrickyBookStore.Services.Payment
{
    public interface IPaymentService
    {
        double GetPaymentAmount(PaymentAmountSearchParameter searchParameter);
    }
}
