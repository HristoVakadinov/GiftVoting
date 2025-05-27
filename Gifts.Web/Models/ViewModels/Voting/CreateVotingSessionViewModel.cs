using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Web.Models.ViewModels.Voting
{
    public class CreateVotingSessionViewModel
    {
        public int BirthdayPersonId { get; set; }
        public string BirthdayPersonName { get; set; }
    }
}