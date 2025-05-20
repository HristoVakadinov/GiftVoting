using Gifts.Models;
using Gifts.Repository.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gifts.Repository.Interfaces.Gift
{
    public interface IGiftRepository : IBaseRepository<Models.Gift, GiftFilter, GiftUpdate>
    {

    }
} 