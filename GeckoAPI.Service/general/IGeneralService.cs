using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.general
{
    public interface IGeneralService
    {
        Task<List<Country>> GetAllCountries();
        Task<List<State>> GetStatesByCountryId(long countryId);
        Task<List<City>> GetCitiesByStateId(long stateId);
    }
}
