using System.ComponentModel.DataAnnotations;

namespace Gifts.Models
{
    public class Vote
    {
        public int VoteId { get; set; }

        [Required(ErrorMessage = "Voting session is required")]
        public int VotingSessionId { get; set; }

        [Required(ErrorMessage = "Voter is required")]
        public int VoterId { get; set; }

        [Required(ErrorMessage = "Gift is required")]
        public int GiftId { get; set; }

        [Required(ErrorMessage = "Vote date is required")]
        [DataType(DataType.DateTime)]
        public DateTime VoteDate { get; set; }
    }
}
