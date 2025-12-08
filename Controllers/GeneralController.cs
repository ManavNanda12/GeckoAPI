using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.general;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.Controllers
{
    [Route("api/general")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        #region Fields
        private readonly IGeneralService _generalService;
        #endregion

        #region Constructor
        public GeneralController(IGeneralService generalService)
        {
            _generalService = generalService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get country list
        /// </summary>
        [HttpGet("get-country-list")]
        public async Task<BaseAPIResponse<List<Country>>> GetCountryList()
        {
            var response = new BaseAPIResponse<List<Country>>();
            try
            {
                var countryList = await _generalService.GetAllCountries();

                // Set the response data
                response.Data = countryList;
                response.Success = true;
                response.Message = "Country list data fetched successfully.";
            }
            catch (Exception ex)
            {
                // Handle exceptions and set error response
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// Get State list by Country Id
        /// </summary>
        [HttpGet("get-state-list/{CountryId}")]
        public async Task<BaseAPIResponse<List<State>>> GetStateList(long CountryId)
        {
            var response = new BaseAPIResponse<List<State>>();
            try
            {
                var stateList = await _generalService.GetStatesByCountryId(CountryId);

                // Set the response data
                response.Data = stateList;
                response.Success = true;
                response.Message = "State list data fetched successfully.";
            }
            catch (Exception ex)
            {
                // Handle exceptions and set error response
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// Get City list by State Id
        /// </summary>
        [HttpGet("get-city-list/{StateId}")]
        public async Task<BaseAPIResponse<List<City>>> GetCityList(long StateId)
        {
            var response = new BaseAPIResponse<List<City>>();
            try
            {
                var cityList = await _generalService.GetCitiesByStateId(StateId);

                // Set the response data
                response.Data = cityList;
                response.Success = true;
                response.Message = "City list data fetched successfully.";
            }
            catch (Exception ex)
            {
                // Handle exceptions and set error response
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        #endregion

    }
}
