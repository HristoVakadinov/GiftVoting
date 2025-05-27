
namespace Gifts.Web.Models.ViewModels.Gift
{
    public class GiftViewModel
    {
        public int GiftId { get; set; }
        public string GiftName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }

    public class GiftListViewModel
    {
        public List<GiftViewModel> Gifts { get; set; }
        public int TotalCount { get; set; }
    }
}