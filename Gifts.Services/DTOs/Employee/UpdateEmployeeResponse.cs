using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.Employee
{
    public class UpdateEmployeeResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}