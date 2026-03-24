using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Model.models
{
    public class Wishlist
    {
    }

    public class WishlistSaveRequestModel
    {
        public long CustomerId { get; set; }
        public long ProductId { get; set; }
    }
}
