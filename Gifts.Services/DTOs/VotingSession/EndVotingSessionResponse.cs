using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.VotingSession
{
    public class EndVotingSessionResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public DateTime? EndedAt { get; set; }
    }
}