using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Web.Models.ViewModels.Voting
{
    public class VotingSessionViewModel
    {
        public int VotingSessionId { get; set; }
        public string BirthdayPersonName { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime? EndDate { get; set; }
        public int VotesCount { get; set; }
        public int CreatedById { get; set; }
        public bool HasUserVoted { get; set; }
        public bool IsCreator { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class VotingSessionListViewModel
    {
        public List<VotingSessionViewModel> ActiveVotingSessions { get; set; }
        public int TotalCount { get; set; }
    }
}