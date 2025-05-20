using Gifts.Repository.Interfaces.Employee;
using Gifts.Services.DTOs.Employee;
using Gifts.Services.Interfaces.Employee;
using Gifts.Models;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using Gifts.Services.Helpers;

namespace Gifts.Services.Implementations.Employee
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        private EmployeeDto MapToDto(Models.Employee employee)
        {
            return new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                Username = employee.Username,
                FullName = employee.FullName,
                BirthDate = employee.BirthDate
            };
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employeeList = await _employeeRepository.RetrieveCollectionAsync(new EmployeeFilter()).ToListAsync();
            if (employeeList == null)
            {
                throw new KeyNotFoundException("No employees found.");
            }
            return employeeList.Select(MapToDto);
        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _employeeRepository.RetrieveAsync(employeeId);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");
            }
            return MapToDto(employee);
        }

        public async Task<bool> UpdateFullNameAsync(int employeeId, string newFullName)
        {
            if (string.IsNullOrEmpty(newFullName))
            {
                throw new ValidationException("Full name cannot be empty.");
            }
            var update = new EmployeeUpdate
            {
                FullName = new SqlString(newFullName)
            };
            return await _employeeRepository.UpdateAsync(employeeId, update);
        }
        public async Task<bool> UpdatePasswordAsync(int employeeId, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ValidationException("Password cannot be empty.");
            }
            var hashedPassword = SecurityHelper.HashPassword(newPassword);
            var update = new EmployeeUpdate
            {
                Password = new SqlString(hashedPassword)
            };
            return await _employeeRepository.UpdateAsync(employeeId, update);
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesWithUpcomingBirthdays(int daysAhead)
        {
            var employees = await _employeeRepository.RetrieveCollectionAsync(new EmployeeFilter()).ToListAsync();
            if (employees == null)
            {
                throw new KeyNotFoundException("No employees found.");
            }
            var today = DateTime.Today;
            var upcomingBirthdays = employees.Where(e => 
            {
                var nextBirthday = new DateTime(today.Year, e.BirthDate.Month, e.BirthDate.Day);
                if(nextBirthday < today)
                {
                    nextBirthday = nextBirthday.AddYears(1);
                }
                var daysUntilBirthday = (nextBirthday - today).Days;
                return daysUntilBirthday <= daysAhead;
            });

            return upcomingBirthdays.Select(MapToDto);
        }        
    }
}