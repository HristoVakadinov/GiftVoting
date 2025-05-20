using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gifts.Repository.Interfaces.Employee
{
    public class EmployeeFilter
    {
        public SqlString? Username { get; set; }
    }
}
