using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.address;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.CustomerControllers
{
    [Route("api/customer/address")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        #region Fields
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAddressService _addressService;
        #endregion
        #region Constructor
        public AddressController(IHttpContextAccessor httpContextAccessor , IAddressService addressService)
        {
            _httpContextAccessor = httpContextAccessor;
            _addressService = addressService;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Save Address
        /// </summary>        
        [HttpPost("save-address")]
        public async Task<BaseAPIResponse<long>> SaveAddress(Address model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {

                var isAdded = await _addressService.SaveAddress(model);
                if (isAdded == 0)
                {
                    response.Success = true;
                    response.Message = "Address updated successfully.";
                }
                else if (isAdded > 0)
                {
                    response.Success = true;
                    response.Message = "Address saved successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Failed to save address.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// Get address list
        /// </summary>
        [HttpGet("get-address-list/{CustomerId}")]
        public async Task<BaseAPIResponse<List<AddressListResponseModel>>> GetAddressList(long CustomerId)
        {
            var response = new BaseAPIResponse<List<AddressListResponseModel>>();
            try
            {
                var addresses = await _addressService.GetAddressList(CustomerId);
                response.Data = addresses;
                response.Success = true;
                response.Message = "Address fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }
        #endregion
    }
}
