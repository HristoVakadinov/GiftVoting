using Gifts.Repository.Interfaces.Employee;
using Gifts.Services.DTOs.Employee;
using Gifts.Services.Interfaces.Employee;
using Gifts.Models;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using Gifts.Services.Helpers;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualBasic;

namespace Gifts.Services.Implementations.Employee
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<GetAllEmployeesResponse> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.RetrieveCollectionAsync(new EmployeeFilter()).ToListAsync();
            var today = DateTime.Today;
            var employeeInfo = employees.Select(e =>
            {
                var nextBirthday = new DateTime(
                    today.Year,
                    e.BirthDate.Month,
                    e.BirthDate.Day
                );
                if (nextBirthday < today)
                {
                    nextBirthday = nextBirthday.AddYears(1);
                }
                
                var daysToNextBirthday = (nextBirthday - today).Days;

                return new EmployeeInfo
                {
                    EmployeeId = e.EmployeeId,
                    FullName = e.FullName,
                    DaysTillNextBirthday = daysToNextBirthday,
                    BirthDate = e.BirthDate
                };
            }).ToList();

            return new GetAllEmployeesResponse
            {
                Employees = employeeInfo,
                TotalCount = employeeInfo.Count
            };
        }

        public async Task<GetEmployeeResponse> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _employeeRepository.RetrieveAsync(employeeId);
            if (employee == null)
            {
                throw new Exception("Employee not found");
            }
            return new GetEmployeeResponse
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                BirthDate = employee.BirthDate,
                Username = employee.Username
            };
        }

        public async Task<GetAllEmployeesResponse> GetEmployeesWithUpcomingBirthdays(int daysAhead)
        {
            var allEmployees = await GetAllEmployeesAsync();

            var today = DateTime.Today;
            var filteredEmployees = allEmployees.Employees
                .Where(e => e.DaysTillNextBirthday <= daysAhead)
                .OrderBy(e => e.DaysTillNextBirthday)
                .ToList();

            return new GetAllEmployeesResponse
            {
                Employees = filteredEmployees,
                TotalCount = filteredEmployees.Count()
            };
        }

        public async Task<UpdateEmployeeResponse> UpdateFullNameAsync(UpdateFullNameRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.NewFullName))
                {
                    throw new ValidationException("Full name is required");
                }

                var update = new EmployeeUpdate
                {
                    FullName = new SqlString(request.NewFullName)
                };

                var success = await _employeeRepository.UpdateAsync(request.EmployeeId, update);
                
                return new UpdateEmployeeResponse
                {
                    Success = success,
                    Message = success ? "Full name updated successfully" : "Failed to update full name",
                    UpdatedAt = DateTime.Now
                };                
            }
            catch (Exception ex)
            {
                return new UpdateEmployeeResponse
                {
                    Success = false,
                    Message = ex.Message,
                    UpdatedAt = DateTime.Now
                };
            }
        }

        public async Task<UpdateEmployeeResponse> UpdatePasswordAsync(UpdatePasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.NewPassword))
                    throw new ValidationException("Password cannot be empty");

                var hashedPassword = SecurityHelper.HashPassword(request.NewPassword);
                var update = new EmployeeUpdate
                {
                    Password = new SqlString(hashedPassword)
                };

                var success = await _employeeRepository.UpdateAsync(request.EmployeeId, update);

                return new UpdateEmployeeResponse
                {
                    Success = success,
                    UpdatedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                return new UpdateEmployeeResponse
                {
                    Success = false,
                    Message = ex.Message,
                    UpdatedAt = DateTime.Now
                };
            }
        }
    }
}