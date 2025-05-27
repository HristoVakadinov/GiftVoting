using Gifts.Services.Interfaces.Gift;
using Gifts.Web.Attributes;
using Gifts.Web.Models.ViewModels.Gift;
using Microsoft.AspNetCore.Mvc;

namespace Gifts.Web.Controllers.Gift
{
    [Authorize]
    public class GiftController : Controller
    {
        private readonly IGiftService _giftService;

        public GiftController(IGiftService giftService)
        {
            _giftService = giftService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _giftService.GetAllGiftsAsync();
            var viewModel = new GiftListViewModel
            {
                Gifts = response.Gifts.Select(g => new GiftViewModel
                {
                    GiftId = g.GiftId,
                    GiftName = g.Name,
                    Description = g.Description,
                    Price = g.Price
                }).ToList(),
                TotalCount = response.TotalCount
            };
            return View(viewModel);
        }
    }
}