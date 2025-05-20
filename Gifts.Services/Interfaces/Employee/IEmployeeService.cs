using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gifts.Services.DTOs.Employee;

namespace Gifts.Services.Interfaces.Employee
{
    public interface IEmployeeService
    {
        Task<EmployeeDto> GetEmployeeByIdAsync(int employeeId);
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        // Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employee);
         Task<bool> UpdateFullNameAsync(int employeeId, string newFullName);
        Task<bool> UpdatePasswordAsync(int employeeId, string newPassword);
        Task<IEnumerable<EmployeeDto>> GetEmployeesWithUpcomingBirthdays(int daysAhead);
    }
}