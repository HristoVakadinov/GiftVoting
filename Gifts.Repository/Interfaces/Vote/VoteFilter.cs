using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gifts.Repository.Interfaces.Vote
{
    public class VoteFilter
    {
        public SqlInt32? VoterId { get; set; }
        public SqlInt32? VotingSessionId { get; set; }
    }
}
