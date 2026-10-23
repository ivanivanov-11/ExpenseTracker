using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Services.DTOs.Expense.Response
{
    public class ExpensePageResponse
    {
        public IEnumerable<ExpenseResponse> Expenses { get; set; } = new List<ExpenseResponse>();
        public decimal Total { get; set; }
    }
}
