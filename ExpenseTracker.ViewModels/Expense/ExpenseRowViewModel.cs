using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.ViewModels.Expense
{
    public class ExpenseRowViewModel
    {
        // trusted -> untrusted
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public string CategoryName { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
    }
}
