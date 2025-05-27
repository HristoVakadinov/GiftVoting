using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gifts.Services.DTOs.Employee;

namespace Gifts.Services.Interfaces.Employee
{
    public interface IEmployeeService
    {
        Task<GetEmployeeResponse> GetEmployeeByIdAsync(int employeeId);
        Task<GetAllEmployeesResponse> GetAllEmployeesAsync();
        // Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employee);
        Task<UpdateEmployeeResponse> UpdateFullNameAsync(UpdateFullNameRequest request);
        Task<UpdateEmployeeResponse> UpdatePasswordAsync(UpdatePasswordRequest request);
        Task<GetAllEmployeesResponse> GetEmployeesWithUpcomingBirthdays(int daysAhead);
    }
}