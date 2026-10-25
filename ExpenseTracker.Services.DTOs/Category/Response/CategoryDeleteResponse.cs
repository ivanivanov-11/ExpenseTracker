using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Services.DTOs.Category.Response
{
    public class CategoryDeleteResponse
    {
        public bool Success { get; set; }
        public int UpdatedExpensesCount { get; set; }
    }
}
