using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Model.models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string AddressName { get; set; }
        public string FullAddress { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public bool? IsDefault { get; set; }
        public int? UpdatedBy { get; set; }
        public long CustomerId { get; set; }
    }

    public class AddressListResponseModel
    {
        public int AddressId { get; set; }
        public string AddressName { get; set; }
        public string FullAddress { get; set; }
        public bool IsDefault { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public long CityId { get; set; }

    }
}
