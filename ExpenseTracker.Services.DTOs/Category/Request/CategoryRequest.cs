using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Services.DTOs.Category.Request
{
    public class CategoryRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
