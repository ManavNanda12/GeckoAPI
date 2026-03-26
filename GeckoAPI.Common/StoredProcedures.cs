using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebAPI.Common
{
    public class StoredProcedures
    {
        #region User    
        public const string GetAllUsers = "SP_GetUsers";
        public const string GetUserById = "SP_GetUserById";
        public const string DeleteUser = "SP_DeleteUser";
        public const string SaveUser = "SP_SaveUser";
        public const string GetUserByEmail = "SP_GetUserByEmail";
        #endregion

        #region UserTokenCheck
        public const string AddUserToken = "SP_InsertUserToken";
        public const string ValidateToken = "SP_ValidateUserToken";
        public const string GetUserDataByJwtToken = "SP_GetUserDataByJWT";
        #endregion

        #region UserAuth
        public const string SaveLoginAttemptLogs = "SP_SaveLoginAttemptLogs";
        public const string GetAttemptedLogs = "SP_GetAttemptedLogs";
        public const string ChangeLockStatus = "SP_ChangeLockStatus";
        #endregion

        #region Category
        public const string SaveCategory = "SP_SaveCategory";
        public const string SaveCategoryImage = "SP_SaveCategoryImage";
        public const string GetCategoryList = "SP_GetCategoryList";
        public const string DeleteCategoryImage = "SP_DeleteCategoryImage";
        public const string GetCategoryImageById = "SP_GetCategoryImageById";
        #endregion

        #region Products
        public const string GetProductList = "SP_GetProductList";
        public const string SaveProduct = "SP_SaveProducts";
        public const string SaveProductImage = "SP_SaveProductImage";
        public const string GetProductListByCategoryId = "SP_GetProductListByCategoryId";
        public const string GetProductImages = "SP_GetProductImages";
        public const string DeleteProductImages = "SP_DeleteProductImages";
        public const string SetPrimaryProductImage = "SP_SetPrimaryProductImage";
        public const string GetProductListByCategoryId_InStockOnly = "SP_GetProductListByCategoryId_InStockOnly";
        #endregion

        #region Product Stock
        public const string GetProductStockDetails = "SP_GetProductStockDetails";
        public const string UpdateProductStock = "SP_UpdateProductStock";
        #endregion

        #region Cart
        public const string AddToCart = "SP_AddToCart";
        public const string GetCartContents = "SP_GetCartContents";
        public const string UpdateCartItemQuantity= "SP_UpdateCartItemQuantity";
        public const string UpdateCartCustomerId = "SP_UpdateCartCustomerId";
        #endregion

        #region Order
        public const string CheckoutCart = "SP_CheckoutCart";
        public const string GetOrderList = "SP_GetOrderList";
        public const string GetOrderDetail = "SP_GetOrderDetail";
        public const string GetAdminOrderList = "SP_GetAdminOrderList";
        public const string SaveStripeWebHookEvents = "SP_SaveStripeWebHookEvents";
        #endregion

        #region Customer
        public const string GetCustomerByEmail = "SP_GetCustomerByEmail";
        public const string GetCustomerById = "SP_GetCustomerById";
        public const string GetCustomerList = "SP_GetCustomerList";
        public const string SaveCustomer = "SP_SaveCustomer";
        public const string UpdateCustomerWelcomeStatus = "SP_UpdateCustomerWelcomeStatus";
        public const string DeleteCustomer = "SP_DeleteCustomer";
        public const string UpdateFCMToken = "SP_UpdateFCMToken";
        public const string GetInactiveCartUsers = "SP_GetInactiveCartUsers";
        public const string InsertCartNotificationLog = "SP_InsertCartNotificationLog";
        #endregion

        #region CustomerTokenCheck
        public const string AddCustomerToken = "SP_InsertCustomerToken";
        public const string ValidateCustomerToken = "SP_ValidateCustomerToken";
        public const string GetCustomerDataByJwtToken = "SP_GetCustomerDataByJWT";
        #endregion

        #region Dashboard
        public const string GetDashboardCounts = "SP_GetDashboardCounts";
        public const string MonhtlySalesAmount = "SP_MonhtlySalesAmount";
        public const string GetMostOrderedProducts = "SP_GetMostOrderedProducts";
        #endregion

        #region ContactUs
        public const string SaveContactRequest = "SP_SaveContactRequest";
        public const string GetContactUsRequestList = "SP_GetContactUsRequestList";
        public const string DeleteContactRequest = "SP_DeleteContactRequest";
        #endregion

        #region Wishlist
        public const string SaveWishlist = "SP_SaveWishlist";
        public const string RemoveFromWhislist = "SP_RemoveFromWhislist";
        public const string GetWishlistProductList = "SP_GetWishlistProductList";
        #endregion

        #region Coupons
        public const string GetCouponList = "SP_GetCouponList";
        public const string SaveCoupon = "SP_SaveCoupon";
        public const string ApplyCoupon = "SP_ApplyCoupon";
        public const string RemoveCoupon = "SP_RemoveCoupon";
        public const string GetCouponUsedDetails = "SP_GetCouponUsedDetails";
        #endregion

        #region SitePolicies
        public const string UpdatePolicy = "SP_UpdatePolicy";
        public const string GetPolicies = "SP_GetPolicies";
        #endregion

        #region Address
        public const string SaveAddress = "SP_SaveAddress";
        public const string GetAddressList = "SP_GetAddressList";
        public const string DeleteAddress = "SP_DeleteAddress";
        public const string MakeCustomerAddressDefault = "SP_MakeCustomerAddressDefault";
        #endregion

        #region General
        public const string GetCountryList = "SP_GetCountryList";
        public const string GetStateList = "SP_GetStateList";
        public const string GetCityList = "SP_GetCityList";
        #endregion

        #region Plan
        public const string GetPlans = "SP_GetPlans";
        public const string SaveCustomerSubscription = "SP_SaveCustomerSubscription";
        public const string CancelSubscription = "SP_CancelSubscription";
        public const string CheckSubscriptionAction = "SP_CheckSubscriptionAction";
        public const string GetPlanSubscriptonHistory = "SP_GetPlanSubscriptionHistory";
        #endregion
    }
}
