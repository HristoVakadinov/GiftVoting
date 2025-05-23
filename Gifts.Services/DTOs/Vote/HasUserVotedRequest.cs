using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.Vote
{
    public class HasUserVotedRequest
    {
        public int VotingSessionId { get; set; }
        public int EmployeeId { get; set; }
    }
}