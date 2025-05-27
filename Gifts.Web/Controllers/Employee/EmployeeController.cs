using Gifts.Services.Interfaces.Employee;
using Gifts.Web.Attributes;
using Gifts.Web.Models.ViewModels.Employee;
using Microsoft.AspNetCore.Mvc;

namespace Gifts.Web.Controllers.Employee
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
        {
            var allEmployees = await _employeeService.GetAllEmployeesAsync();
            var upcomingBirthdays = await _employeeService.GetEmployeesWithUpcomingBirthdays(90);

            var viewModel = new EmployeeListViewModel
            {
                Employees = allEmployees.Employees.Select(e => new EmployeeViewModel
                {
                    EmployeeId = e.EmployeeId,
                    FullName = e.FullName,
                    BirthDate = e.BirthDate,
                    DaysToNextBirthday = e.DaysTillNextBirthday
                }).ToList(),
                TotalCount = allEmployees.TotalCount,
                EmployeesWithUpcomingBirthdays = upcomingBirthdays.Employees.Select(e => new EmployeeViewModel
                {
                    EmployeeId = e.EmployeeId,
                    FullName = e.FullName,
                    BirthDate = e.BirthDate,
                    DaysToNextBirthday = e.DaysTillNextBirthday
                }).ToList()
            };
            return View(viewModel);
        }
    }
}