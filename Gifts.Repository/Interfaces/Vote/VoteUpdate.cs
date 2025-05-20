using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gifts.Repository.Interfaces.Vote
{
    public class VoteUpdate
    {
        public SqlInt32? GiftId { get; set; }
        public SqlDateTime? VoteDate { get; set; }
    }
}
