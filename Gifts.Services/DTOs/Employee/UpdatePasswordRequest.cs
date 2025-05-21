using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.Employee
{
    public class UpdatePasswordRequest
    {
        public int EmployeeId { get; set; }
        public string NewPassword { get; set; }
    }
}