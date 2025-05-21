using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.Vote
{
    public class HasUserVotedResponse
    {
        public bool HasVoted { get; set; }
        public DateTime? VotedAt { get; set; }
        public string? GiftName { get; set; }
    }
}