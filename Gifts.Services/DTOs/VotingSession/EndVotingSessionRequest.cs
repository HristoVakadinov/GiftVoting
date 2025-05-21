using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.VotingSession
{
    public class EndVotingSessionRequest
    {
        public int VotingSessionId { get; set; }
        public int EndedById { get; set; }
    }
}