using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.Employee
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
    }
}