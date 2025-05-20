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

        private GiftDto MapToDto(Models.Gift gift)
        {
            return new GiftDto
            {
                GiftId = gift.GiftId,
                Name = gift.Name,
                Description = gift.Description,
                Price = gift.Price,
            };
        }

        public async Task<IEnumerable<GiftDto>> GetAllGiftsAsync()
        {
            var gifts = await _giftRepository.RetrieveCollectionAsync(new GiftFilter()).ToListAsync();
            return gifts.Select(MapToDto);
        }

        public async Task<GiftDto> GetGiftByIdAsync(int giftId)
        {
            var gift = await _giftRepository.RetrieveAsync(giftId);
            if (gift == null)
            {
                throw new Exception("Gift not found");
            }
            return MapToDto(gift);
        }
        
    }
}