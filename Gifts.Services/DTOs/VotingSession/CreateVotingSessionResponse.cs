using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.VotingSession
{
    public class CreateVotingSessionResponse : VotingSessionInfo
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}