using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.address
{
    public interface IAddressRepository
    {
        Task<List<AddressListResponseModel>> GetAddressList(long customerId);
        Task<long> SaveAddress(Address model);
        Task<long> DefaultAddressChange(long addressId);
        Task<long> DeleteAddress(long addressId);
    }
}
