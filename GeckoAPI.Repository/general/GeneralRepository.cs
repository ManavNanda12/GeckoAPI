using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using Dapper;

namespace GeckoAPI.Repository.general
{
    public class GeneralRepository : BaseRepository, IGeneralRepository
    {
        #region Constructor
        public GeneralRepository(IOptions<DbConfig> config) : base(config)
        {
        }
        #endregion

        #region Methods

        public Task<List<Country>> GetAllCountries()
        {
            var query = GetPgFunctionQuery(StoredProcedures.GetCountryList);
            var response = Query<Country>(query);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<List<State>> GetStatesByCountryId(long countryId)
        {
            var param = new DynamicParameters();
            param.Add("CountryId", countryId);
            var query = GetPgFunctionQuery(StoredProcedures.GetStateList, true, "@CountryId");
            var response = Query<State>(query,param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<List<City>> GetCitiesByStateId(long stateId)
        {
            var param = new DynamicParameters();
            param.Add("StateId", stateId);
            var query = GetPgFunctionQuery(StoredProcedures.GetCityList, true, "@StateId");
            var response = Query<City>(query, param);
            return Task.FromResult(response.Data.ToList());
        }

        #endregion
    }
}