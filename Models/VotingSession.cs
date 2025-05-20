using System.ComponentModel.DataAnnotations;

namespace Gifts.Models
{
    public class VotingSession
    {
        public int VotingSessionId { get; set; }

        [Required(ErrorMessage = "Birthday person is required")]
        public int BirthdayPersonId { get; set; }

        [Required(ErrorMessage = "Session creator is required")]
        public int CreatedById { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [Range(1900, 9999, ErrorMessage = "Invalid birth year")]
        public int BirthYear { get; set; }
    }
}
