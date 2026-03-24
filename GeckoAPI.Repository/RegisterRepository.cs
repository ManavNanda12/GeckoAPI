using DemoWebAPI.Repository.User;
using GeckoAPI.Repository.address;
using GeckoAPI.Repository.cart;
using GeckoAPI.Repository.category;
using GeckoAPI.Repository.contactus;
using GeckoAPI.Repository.coupon;
using GeckoAPI.Repository.customer;
using GeckoAPI.Repository.dashboard;
using GeckoAPI.Repository.general;
using GeckoAPI.Repository.order;
using GeckoAPI.Repository.payment;
using GeckoAPI.Repository.plan;
using GeckoAPI.Repository.product;
using GeckoAPI.Repository.sitepolicy;
using GeckoAPI.Repository.wishlist;
using Microsoft.Extensions.DependencyInjection;

namespace DemoWebAPI.Repository
{
    public static class RegisterRepository
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IContactusRepository, ContactusRepository>();
            services.AddScoped<IWishlistRepository, WishlistRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<ISitePolicyRepository, SitePolicyRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IGeneralRepository, GeneralRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPlanRepository, PlanRepository>();
            // register more repositories here
            return services;
        }
    }
}
