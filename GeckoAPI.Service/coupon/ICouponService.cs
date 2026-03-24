using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.coupon
{
    public interface ICouponService
    {
        Task<long> SaveCoupon(Coupon model);
        Task<List<Coupon>> GetCouponList(CommonListRequestModel model);
        Task<ApplyCouponResult> ApplyCoupon(ApplyCouponRequestModel model);
        Task<long> RemoveCoupon(string CartSessionId);
        Task<List<CouponUsedListResponseModel>> GetUsedCouponDetails(long CouponId);
    }
}
