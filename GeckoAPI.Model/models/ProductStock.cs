using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Model.models
{
    public class ProductStock
    {
        public long StockId { get; set; }
        public long ProductId { get; set; }
        public long Quantity { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
