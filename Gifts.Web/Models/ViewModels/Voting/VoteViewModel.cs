using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gifts.Web.Models.ViewModels.Gift;

namespace Gifts.Web.Models.ViewModels.Voting
{
    public class VoteViewModel
    {
        public int VotingSessionId { get; set; }
        public string BirthdayPersonName { get; set; }
        public List<GiftViewModel> AvailableGifts { get; set; }
        public int SelectedGiftId { get; set; }
    }

}