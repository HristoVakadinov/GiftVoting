using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.Employee
{
    public class EmployeeInfo
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public int DaysTillNextBirthday { get; set; }
        public DateTime BirthDate { get; internal set; }
    }
}