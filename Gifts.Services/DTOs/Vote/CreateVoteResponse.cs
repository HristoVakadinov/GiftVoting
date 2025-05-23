using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.Vote
{
    public class CreateVoteResponse : VoteInfo
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}