using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gifts.Services.DTOs.Gift;

namespace Gifts.Services.Interfaces.Gift
{
    public interface IGiftService
    {
        Task<GiftDto> GetGiftByIdAsync(int giftId);
        Task<IEnumerable<GiftDto>> GetAllGiftsAsync();
        // Task<GiftDto> CreateGiftAsync(GiftDto gift);
        // Task<GiftDto> UpdateGiftAsync(GiftDto gift);
        // Task<bool> DeleteGiftAsync(int giftId);
    }
}