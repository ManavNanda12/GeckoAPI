using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using System.Data;

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
            var query = GetPgFunctionQuery(
               StoredProcedures.GetAddressList,
               true,
               "@CustomerId"
           );
            var data = Query<AddressListResponseModel>(query, param);
            return Task.FromResult(data.Data.ToList());
        }
        public Task<long> SaveAddress(Address model)
        {
            var param = new DynamicParameters();
            param.Add("@AddressId", model.AddressId, DbType.Int64);
            param.Add("@CustomerId", model.CustomerId, DbType.Int64);
            param.Add("@AddressName", model.AddressName);
            param.Add("@FullAddress", model.FullAddress);
            param.Add("@CountryId", model.CountryId, DbType.Int64);
            param.Add("@StateId", model.StateId, DbType.Int64);
            param.Add("@CityId", model.CityId, DbType.Int64);
            param.Add("@UpdatedBy", model.UpdatedBy, DbType.Int64);
            var query = GetPgFunctionQuery(
              StoredProcedures.SaveAddress,
              false,
              "@AddressId,@CustomerId,@AddressName,@FullAddress,@CountryId,@StateId,@CityId,@UpdatedBy"
          );
            var response = Execute(query, param);
            return Task.FromResult((long)response.Data);
        }

        public Task<long> DefaultAddressChange(long addressId)
        {
            var param = new DynamicParameters();
            param.Add("@AddressId", addressId);
            var query = GetPgFunctionQuery(
              StoredProcedures.MakeCustomerAddressDefault,
              false,
              "@AddressId"
          );
            var response = Execute(query, param);
            return Task.FromResult((long)response.Data);

        }

        public Task<long> DeleteAddress(long addressId)
        {
            var param = new DynamicParameters();
            param.Add("@AddressId", addressId);
            var query = GetPgFunctionQuery(
              StoredProcedures.DeleteAddress,
              false,
              "@AddressId"
          );
            var response = Execute(query, param);
            return Task.FromResult((long)response.Data);
        }
        #endregion
    }
}
