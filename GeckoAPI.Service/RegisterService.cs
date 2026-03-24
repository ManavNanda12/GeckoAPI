using DemoWebAPI.Service.User;
using GeckoAPI.Service.address;
using GeckoAPI.Service.cart;
using GeckoAPI.Service.category;
using GeckoAPI.Service.contactus;
using GeckoAPI.Service.coupon;
using GeckoAPI.Service.customer;
using GeckoAPI.Service.dashboard;
using GeckoAPI.Service.general;
using GeckoAPI.Service.jwt;
using GeckoAPI.Service.order;
using GeckoAPI.Service.payment;
using GeckoAPI.Service.plan;
using GeckoAPI.Service.product;
using GeckoAPI.Service.sitepolicy;
using GeckoAPI.Service.wishlist;
using Microsoft.Extensions.DependencyInjection;

namespace DemoWebAPI.Service
{
    public static class RegisterService
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService,CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IContactusService, ContactusService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<ISitePolicyService, SitePolicyService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IGeneralService,GeneralService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPlanService, PlanService>();
            // register more services here
            return services;
        }
    }
}
