using Gifts.Web.Models.ViewModels.Voting;

namespace Gifts.Web.Models.ViewModels.Home
{
    public class HomeViewModel
    {
        public List<VotingSessionViewModel> ActiveVotingSessions { get; set; }
        public int TotalCompletedVotingSessions { get; set; }
    }
}