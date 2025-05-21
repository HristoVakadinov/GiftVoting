using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.VotingSession
{
    public class GetActiveVotingSessionsResponse
    {
        public List<VotingSessionDto>? ActiveSessions { get; set; }
        public int TotalCount { get; set; }
    }
}