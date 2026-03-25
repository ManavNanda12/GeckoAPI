using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
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
            param.Add("@PageNumber", model.PageNumber,DbType.Int32);
            param.Add("@PageSize", model.PageSize, DbType.Int32);
            param.Add("@SearchTerm", model.SearchTerm);
            param.Add("@SortColumn", model.SortColumn);
            param.Add("@SortDirection", model.SortDirection);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetCouponList,
                true,
                "@PageNumber,@PageSize,@SearchTerm,@SortColumn,@SortDirection"
            );

            var response = Query<Coupon>(query, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<long> SaveCoupon(Coupon model)
        {
            var param = new DynamicParameters();
            param.Add("@CouponId", model.CouponId, DbType.Int32);
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
            param.Add("@CreatedBy", model.CreatedBy, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.SaveCoupon,
                false,
                "@CouponId,@CouponCode,@CouponName,@Description,@DiscountType,@DiscountValue,@StartDate,@EndDate,@MaxUsageCount,@MaxUsagePerUser,@IsActive,@CreatedBy"
            );

            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<ApplyCouponResult> ApplyCoupon(ApplyCouponRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CouponCode", model.CouponCode);
            param.Add("@CartSessionId", model.CartSessionId);

            var query = GetPgFunctionQuery(
                StoredProcedures.ApplyCoupon,
                true,
                "@CouponCode,@CartSessionId"
            );

            var response = QueryFirstOrDefault<ApplyCouponResult>(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<long> RemoveCoupon(string CartSessionId)
        {
            var param = new DynamicParameters();
            param.Add("@CartSessionId", CartSessionId);

            var query = GetPgFunctionQuery(
                StoredProcedures.RemoveCoupon,
                false,
                "@CartSessionId"
            );

            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<List<CouponUsedListResponseModel>> GetUsedCouponDetails(long CouponId)
        {
            var param = new DynamicParameters();
            param.Add("@CouponId", CouponId, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetCouponUsedDetails,
                true,
                "@CouponId"
            );

            var response = Query<CouponUsedListResponseModel>(query, param);
            return Task.FromResult(response.Data.ToList());
        }
        #endregion
    }
}
