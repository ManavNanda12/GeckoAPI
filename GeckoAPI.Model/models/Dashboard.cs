using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Model.models
{
    public class Dashboard
    {
        public long Users { get; set; }
        public long Orders { get; set; }
        public long Products { get; set; }
        public long Categories { get; set; }
        public long Customers { get; set; }
    }

    public class MonthlySalesResponseModel
    {
        public long SalesYear { get; set; }
        public long SalesMonth { get; set; }
        public string MonthName { get; set; }
        public decimal MonthlyTotal { get; set; }
    }

    public class MostOrderedProductsResponseModel
    {
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public long OrderedCount { get; set; }
        public string ProductImage { get; set; }
    }

    public class GoogleLoginModel
    {
        public string Token { get; set; }
    }
}
