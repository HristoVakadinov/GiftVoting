using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.VotingSession
{
    public class VotingSessionDto
    {
        public int VotingSessionId { get; set; }
        public int BirthdayPersonId { get; set; }
        public string BirthdayPersonName { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByFullName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public int BirthYear { get; set; }
    }
}