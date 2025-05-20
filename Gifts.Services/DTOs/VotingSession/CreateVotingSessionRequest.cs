using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.VotingSession
{
    public class CreateVotingSessionRequest
    {
        public int BirthdayPersonId { get; set; }
        public int CreatedById { get; set; }
        public DateTime? EndDate { get; set; }
        public int? BirthYear { get; set; }
    }
}