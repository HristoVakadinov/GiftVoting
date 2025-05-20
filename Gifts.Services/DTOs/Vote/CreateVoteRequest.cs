using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.Vote
{
    public class CreateVoteRequest
    {
        public int VotingSessionId { get; set; }
        public int VoterId { get; set; }
        public int GiftId { get; set; }
    }
}