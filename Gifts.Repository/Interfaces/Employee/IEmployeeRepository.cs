using Gifts.Repository.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gifts.Models;

namespace Gifts.Repository.Interfaces.Employee
{
    public interface IEmployeeRepository : IBaseRepository<Models.Employee, EmployeeFilter, EmployeeUpdate>
    {
    }
} 