using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Services.DTOs.Expense.Request
{
    public class ExpenseFilterRequest
    {
        public string? Search { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
    }
}
