using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.Employee
{
    public class GetAllEmployeesResponse
    {
        public List<EmployeeInfo> Employees { get; set; }
        public int TotalCount { get; set; }
    }
}