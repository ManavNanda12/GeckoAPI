using GeckoAPI.Model.models;
using GeckoAPI.Repository.address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.address
{
    public class AddressService: IAddressService
    {
        #region Fields
        public IAddressRepository _addressRepository { get; set; }
        #endregion

        #region Constructor
        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }
        #endregion

        #region Methods
        public async Task<List<AddressListResponseModel>> GetAddressList(long customerId)
        {
            var data = await _addressRepository.GetAddressList(customerId);
            return data;
        }

        public Task<long> SaveAddress(Address model)
        {
           var data =  _addressRepository.SaveAddress(model);
           return data;
        }

        public Task<long> DefaultAddressChange(long addressId)
        {
            var data = _addressRepository.DefaultAddressChange(addressId);
            return data;
        }

        public Task<long> DeleteAddress(long addressId)
        {
            var data = _addressRepository.DeleteAddress(addressId);
            return data;
        }

        #endregion
    }
}
