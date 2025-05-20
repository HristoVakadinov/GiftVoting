using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gifts.Repository.Interfaces.VotingSession
{
    public class VotingSessionFilter
    {
        public SqlInt32? BirthdayPersonId { get; set; }
        public SqlInt32? CreatedById { get; set; }
        public SqlBoolean? IsActive { get; set; }
    }
}
