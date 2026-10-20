using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Services.DTOs.Expense.Request
{
    public class ExpenseRequest
    {
        public string Title { get; set; } = null!;
        public string? Notes { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public Guid CategoryId { get; set; }
        public string PaymentMethod { get; set; } = "Card";
    }
}
