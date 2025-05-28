using Gifts.Repository.Interfaces.Gift;
using Gifts.Services.DTOs.Gift;
using Gifts.Services.Interfaces.Gift;

namespace Gifts.Services.Implementations.Gift
{
    public class GiftService : IGiftService
    {
        private readonly IGiftRepository _giftRepository;

        public GiftService(IGiftRepository giftRepository)
        {
            _giftRepository = giftRepository;
        }

        private GiftInfo MapToGiftInfo(Models.Gift gift)
        {
            return new GiftInfo
            {
                GiftId = gift.GiftId,
                Name = gift.Name,
                Description = gift.Description,
                Price = gift.Price
            };
        }

        public async Task<GetAllGiftsResponse> GetAllGiftsAsync()
        {
            var gifts = await _giftRepository.RetrieveCollectionAsync(new GiftFilter()).ToListAsync();
            var giftsResponse = new GetAllGiftsResponse
            {
                Gifts = gifts.Select(MapToGiftInfo).ToList(),
                TotalCount = gifts.Count
            };
            return giftsResponse;
        }

        public async Task<GetGiftResponse> GetGiftByIdAsync(int giftId)
        {
            try
            {
                var gift = await _giftRepository.RetrieveAsync(giftId);
                return (GetGiftResponse)MapToGiftInfo(gift);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}