using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.Gift
{
    public class GetAllGiftsResponse
    {
        public List<GiftInfo>? Gifts { get; set; }
        public int TotalCount { get; set; }
    }

}