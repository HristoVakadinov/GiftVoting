using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Web.Models.ViewModels.Employee
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public int DaysToNextBirthday { get; set; }
    }

    public class EmployeeListViewModel
    {
        public List<EmployeeViewModel> Employees { get; set; }
        public int TotalCount { get; set; }
        public List<EmployeeViewModel> EmployeesWithUpcomingBirthdays { get; set; }
    }
}