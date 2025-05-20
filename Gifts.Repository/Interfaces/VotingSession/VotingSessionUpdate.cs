using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gifts.Repository.Interfaces.VotingSession
{
    public class VotingSessionUpdate
    {
        public SqlDateTime? StartDate { get; set; }
        public SqlDateTime? EndDate { get; set; }
        public SqlBoolean? IsActive { get; set; }
    }
}
