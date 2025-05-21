using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.Employee
{
    public class UpdateFullNameRequest
    {
        public int EmployeeId { get; set; }
        public string NewFullName { get; set; }
    }
}