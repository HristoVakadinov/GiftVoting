using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Web.Models.ViewModels.Gift
{
    public class GiftViewModel
    {
        public int GiftId { get; set; }
        public string GiftName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}