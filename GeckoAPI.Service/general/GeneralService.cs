using GeckoAPI.Model.models;
using GeckoAPI.Repository.general;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.general
{
    public class GeneralService : IGeneralService
    {
        #region Fields
        public readonly IGeneralRepository _generalRepository;
        #endregion

        #region Constructor
        public GeneralService(IGeneralRepository generalRepository)
        {
            _generalRepository = generalRepository;
        }
        #endregion

        #region Methods
        public async Task<List<Country>> GetAllCountries()
        {
            var data = await _generalRepository.GetAllCountries();
            return data;
        }

        public Task<List<City>> GetCitiesByStateId(long stateId)
        {
            var data =  _generalRepository.GetCitiesByStateId(stateId);
            return data;
        }

        public Task<List<State>> GetStatesByCountryId(long countryId)
        {
            var data = _generalRepository.GetStatesByCountryId(countryId);
            return data;
        }
        #endregion
    }
}
