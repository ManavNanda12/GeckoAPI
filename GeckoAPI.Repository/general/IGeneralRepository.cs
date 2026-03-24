using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.general
{
    public interface IGeneralRepository
    {
        Task<List<Country>> GetAllCountries();
        Task<List<State>> GetStatesByCountryId(long countryId);
        Task<List<City>> GetCitiesByStateId(long stateId);
    }
}
