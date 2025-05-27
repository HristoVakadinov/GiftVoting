using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Web.Models.ViewModels.Voting
{
    public class VoteResultViewModel
    {
        public string GiftName { get; set; }
        public int VotesCount { get; set; }
        public double Percentage { get; set; }
    }

    public class SessionResultViewModel
    {
        public int SessionId { get; set; }
        public string BirthdayPersonName { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalVotes { get; set; }
        public List<VoteResultViewModel> VoteResults { get; set; }
    }
}