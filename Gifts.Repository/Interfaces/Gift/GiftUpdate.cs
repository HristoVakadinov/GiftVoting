using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gifts.Repository.Interfaces.Gift
{
    public class GiftUpdate
    {
        public SqlString? Name { get; set; }
        public SqlString? Desription { get; set; }
        public SqlDecimal? Price { get; set; }
    }
}
