using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using Npgsql;
using Dapper;

namespace GeckoAPI.Repository.general
{
    public class GeneralRepository : BaseRepository, IGeneralRepository
    {
        private readonly string _connectionString;

        #region Constructor
        public GeneralRepository(IOptions<DbConfig> config) : base(config)
        {
            _connectionString = config.Value.DefaultConnection;
        }
        #endregion

        #region Methods

        public Task<List<Country>> GetAllCountries()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            string sql = $"SELECT * FROM {StoredProcedures.GetCountryList.ToLower()}()";
            var data = connection.Query<Country>(sql).ToList();
            return Task.FromResult(data);
        }

        public Task<List<State>> GetStatesByCountryId(long countryId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var param = new DynamicParameters();
            param.Add("CountryId", countryId);
            string sql = $"SELECT * FROM {StoredProcedures.GetStateList.ToLower()}(@CountryId)";
            var data = connection.Query<State>(sql, param).ToList();
            return Task.FromResult(data);
        }

        public Task<List<City>> GetCitiesByStateId(long stateId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var param = new DynamicParameters();
            param.Add("StateId", stateId);
            string sql = $"SELECT * FROM {StoredProcedures.GetCityList.ToLower()}(@StateId)";
            var data = connection.Query<City>(sql, param).ToList();
            return Task.FromResult(data);
        }

        #endregion
    }
}