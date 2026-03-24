using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.address
{
    public class AddressRepository : BaseRepository , IAddressRepository
    {
        #region Constructor
        public AddressRepository(IOptions<DbConfig> config) : base(config)
        {
        }
        #endregion
        #region Methods
        public Task<List<AddressListResponseModel>> GetAddressList(long customerId)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", customerId);
            var data = Query<AddressListResponseModel>(StoredProcedures.GetAddressList, param);
            return Task.FromResult(data.Data.ToList());
        }
        public Task<long> SaveAddress(Address model)
        {
            var param = new DynamicParameters();
            param.Add("@AddressId", model.AddressId);
            param.Add("@AddressName", model.AddressName);
            param.Add("@FullAddress", model.FullAddress);
            param.Add("@CustomerId", model.CustomerId);
            param.Add("@CountryId", model.CountryId);
            param.Add("@StateId", model.StateId);
            param.Add("@CityId", model.CityId);
            param.Add("@UpdatedBy", model.UpdatedBy);
            var response = Execute(StoredProcedures.SaveAddress, param);
            return Task.FromResult((long)response.Data);
        }

        public Task<long> DefaultAddressChange(long addressId)
        {
            var param = new DynamicParameters();
            param.Add("@AddressId", addressId);
            var response = Execute(StoredProcedures.MakeCustomerAddressDefault, param);
            return Task.FromResult((long)response.Data);

        }

        public Task<long> DeleteAddress(long addressId)
        {
            var param = new DynamicParameters();
            param.Add("@AddressId", addressId);
            var response = Execute(StoredProcedures.DeleteAddress, param);
            return Task.FromResult((long)response.Data);
        }
        #endregion
    }
}
