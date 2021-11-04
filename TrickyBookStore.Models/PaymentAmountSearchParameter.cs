using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// KeepIt
namespace TrickyBookStore.Models
{
    public class PaymentAmountSearchParameter
    {
        public int CustomerId { get; set; }
        public DateTimeOffset FromDate { get; set; }
        public DateTimeOffset ToDate { get; set; }
    }
}
