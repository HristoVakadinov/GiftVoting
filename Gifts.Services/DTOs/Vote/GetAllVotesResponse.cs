using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.Vote
{
    public class GetAllVotesResponse
    {
        public List<VoteInfo>? Votes { get; set; }
        public int TotalCount { get; set; }
    }
}