using GeckoAPI.Model.models;
using GeckoAPI.Repository.coupon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.coupon
{
    public class CouponService:ICouponService
    {

        #region Fields
        public readonly ICouponRepository _couponRepository;
        #endregion
        #region Constructor
        public CouponService(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }
        #endregion

        #region Methods
        public Task<List<Coupon>> GetCouponList(CommonListRequestModel model)
        {
            var data = _couponRepository.GetCouponList(model);
            return data;
        }

        public Task<long> SaveCoupon(Coupon model)
        {
            var data = _couponRepository.SaveCoupon(model);
            return data;
        }
        public Task<ApplyCouponResult> ApplyCoupon(ApplyCouponRequestModel model)
        {
            var data = _couponRepository.ApplyCoupon(model);
            return data;
        }

        public Task<long> RemoveCoupon(string CartSessionId)
        {
            var data = _couponRepository.RemoveCoupon(CartSessionId);
            return data;
        }

        public Task<List<CouponUsedListResponseModel>> GetUsedCouponDetails(long CouponId)
        {
            var data = _couponRepository.GetUsedCouponDetails(CouponId);
            return data;
        }
        #endregion
    }
}
