using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gifts.Repository.Interfaces.Employee;
using Gifts.Services.DTOs.Authentication;
using Gifts.Services.Helpers;
using Gifts.Services.Interfaces.Authentication;
using Gifts.Services.Interfaces.Employee;

namespace Gifts.Services.Implementations.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public AuthenticationService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<LoginResponce> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return new LoginResponce
                {
                    Success = false,
                    Message = "Username and password are required"
                };
            }

            var hashedPassword = SecurityHelper.HashPassword(request.Password);
            var employees = await _employeeRepository.RetrieveCollectionAsync(new EmployeeFilter { Username = request.Username }).ToListAsync();
            var employee = employees.FirstOrDefault();
            
            if (employee == null || employee.Password != hashedPassword)
            {
                return new LoginResponce
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            return new LoginResponce
            {
                Success = true,
                Message = "Login successful",
                FullName = employee.FullName,
                UserId = employee.EmployeeId
            };
        }
        
    }
}