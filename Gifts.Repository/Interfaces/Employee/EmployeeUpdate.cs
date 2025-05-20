using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gifts.Repository.Interfaces.Employee
{
    public class EmployeeUpdate
    {
        public SqlString? FullName { get; set; }

        public SqlString? Password { get; set; }
    }
}
