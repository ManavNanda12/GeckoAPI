using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.coupon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        #region Fields
        private readonly ICouponService _couponService;
        #endregion
        #region Constructor
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get coupon list
        /// </summary>
        [HttpPost("get-coupon-list")]
        public async Task<BaseAPIResponse<List<Coupon>>> GetCouponList(CommonListRequestModel model)
        {
            var response = new BaseAPIResponse<List<Coupon>>();
            try
            {
                var couponList = await _couponService.GetCouponList(model);

                // Set the response data
                response.Data = couponList;
                response.Success = true;
                response.Message = "Coupon data fetched successfully.";
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
        /// Save coupon
        /// </summary>        
        [HttpPost("save-coupon")]
        public async Task<BaseAPIResponse<long>> SaveCoupon(Coupon model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {

                var isAdded = await _couponService.SaveCoupon(model);
                if (isAdded > 0)
                {
                    response.Success = true;
                    response.Message = "Coupon saved succesfully.";
                  
                }
                else if (isAdded == 0)
                {
                    response.Success = true;
                    response.Message = "Coupon updated succesfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Failed to save coupon.";
                }
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
