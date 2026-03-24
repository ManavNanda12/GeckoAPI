using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.coupon
{
    public class CouponRepository:BaseRepository, ICouponRepository
    {
        #region Constructor
        public CouponRepository(IOptions<DbConfig> config) : base(config)
        {
        }
        #endregion

        #region Methods
        public Task<List<Coupon>> GetCouponList(CommonListRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@PageNumber", model.PageNumber);
            param.Add("@PageSize", model.PageSize);
            param.Add("@SearchTerm", model.SearchTerm);
            param.Add("@SortColumn", model.SortColumn);
            param.Add("@SortDirection", model.SortDirection);
            var coupons = Query<Coupon>(StoredProcedures.GetCouponList, param);
            return Task.FromResult(coupons.Data.ToList());
        }

        public Task<long> SaveCoupon(Coupon model)
        {
            var param = new DynamicParameters();
            param.Add("@CouponId", model.CouponId);
            param.Add("@CouponCode", model.CouponCode);
            param.Add("@CouponName", model.CouponName);
            param.Add("@Description", model.Description);
            param.Add("@DiscountType", model.DiscountType);
            param.Add("@DiscountValue", model.DiscountValue);
            param.Add("@StartDate", model.StartDate);
            param.Add("@EndDate", model.EndDate);
            param.Add("@MaxUsageCount", model.MaxUsageCount);
            param.Add("@MaxUsagePerUser", model.MaxUsagePerUser);
            param.Add("@IsActive", model.IsActive);
            param.Add("@CreatedBy", model.CreatedBy);
            var response = Execute(StoredProcedures.SaveCoupon, param);
            return Task.FromResult(response.Data);
        }

        public Task<ApplyCouponResult> ApplyCoupon(ApplyCouponRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CouponCode", model.CouponCode);
            param.Add("@CartSessionId", model.CartSessionId);
            var response = QueryFirstOrDefault<ApplyCouponResult>(StoredProcedures.ApplyCoupon, param);
            return Task.FromResult(response.Data);
        }

        public Task<long> RemoveCoupon(string CartSessionId)
        {
            var param = new DynamicParameters();
            param.Add("@CartSessionId", CartSessionId);
            var response = Execute(StoredProcedures.RemoveCoupon, param);
            return Task.FromResult(response.Data);
        }

        public Task<List<CouponUsedListResponseModel>> GetUsedCouponDetails(long CouponId)
        {
            var param = new DynamicParameters();
            param.Add("@CouponId", CouponId);
            var coupons = Query<CouponUsedListResponseModel>(StoredProcedures.GetCouponUsedDetails, param);
            return Task.FromResult(coupons.Data.ToList());
        }
        #endregion
    }
}
