using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.Vote
{
    public class VoteInfo
    {
        public int VoteId { get; set; }
        public int VotingSessionId { get; set; }
        public int VoterId { get; set; }
        public string VoterName { get; set; }
        public int GiftId { get; set; }
        public string GiftName { get; set; }
        public DateTime VoteDate { get; set; }
    }
}