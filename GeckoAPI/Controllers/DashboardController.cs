using DemoWebAPI.model.Models;
using DemoWebAPI.Service.User;
using GeckoAPI.Common;
using GeckoAPI.Model.models;
using GeckoAPI.Service.dashboard;
using GeckoAPI.Service.jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace GeckoAPI.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {

        #region Fields
        private readonly IDashboardService _dashboardService;
        private readonly IJWTService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public DashboardController(IDashboardService dashboardService, IJWTService jwtService, IConfiguration configuration, EmailService emailService , IWebHostEnvironment env)
        {
            _dashboardService = dashboardService;
            _jwtService = jwtService;
            _configuration = configuration;
            _emailService = emailService;
            _env = env;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get dashboard count
        /// </summary>
        [HttpGet("get-dashboard-count")]
        public async Task<BaseAPIResponse<Dashboard>> GetDashboardCount()
        {
            var response = new BaseAPIResponse<Dashboard>();
            try
            {
                // Fetch dashboard count using the service
                var dashboard = await _dashboardService.GetDashboardCount();

                // Set the response data
                response.Data = dashboard;
                response.Success = true;
                response.Message = "Dashboard count fetched successfully.";
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
        /// Get monthly sale stats
        /// </summary>
        [HttpGet("get-monthly-stats/{Year}")]
        public async Task<BaseAPIResponse<List<MonthlySalesResponseModel>>> GetMonthlySalesStats(long Year)
        {
            var response = new BaseAPIResponse<List<MonthlySalesResponseModel>>();
            try
            {
                // Fetch dashboard count using the service
                var data = await _dashboardService.GetMonthlySalesStats(Year);

                // Set the response data
                response.Data = data;
                response.Success = true;
                response.Message = "Dashboard monthly sales data fetched successfully.";
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
        /// Get most ordered product stats
        /// </summary>
        [HttpGet("get-most-ordered-stats/{Filter}")]
        public async Task<BaseAPIResponse<List<MostOrderedProductsResponseModel>>> GetMostOrderedProductStats(long Filter)
        {
            var response = new BaseAPIResponse<List<MostOrderedProductsResponseModel>>();
            try
            {
                // Fetch dashboard count using the service
                var data = await _dashboardService.GetMostOrderedProductStats(Filter);

                // Set the response data
                response.Data = data;
                response.Success = true;
                response.Message = "Dashboard most ordered products data fetched successfully.";
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
        /// Send monthly reports to admin
        /// </summary>
        [HttpGet("send-monthly-reports")]
        public async Task<BaseAPIResponse<object>> SendMonthlyReports()
        {
            var response = new BaseAPIResponse<object>();

            try
            {
                // Get current date information
                var currentDate = DateTime.Now;
                var currentYear = currentDate.Year;
                var currentMonth = currentDate.Month;
                var lastMonth = currentDate.AddMonths(-1);

                // Fetch monthly sales data for current year
                var monthlySalesData = await _dashboardService.GetMonthlySalesStats(currentYear);

                // Fetch top 10 most ordered products (Filter: 2 = This Year)
                var topProducts = await _dashboardService.GetMostOrderedProductStats(2);

                // Get current and last month sales
                var currentMonthData = monthlySalesData.FirstOrDefault(x => x.SalesMonth == currentMonth);
                var lastMonthData = monthlySalesData.FirstOrDefault(x => x.SalesMonth == lastMonth.Month);

                if (currentMonthData == null)
                {
                    response.Success = false;
                    response.Message = "No sales data found for current month.";
                    return response;
                }

                // Calculate values
                decimal currentMonthSales = currentMonthData.MonthlyTotal;
                decimal lastMonthSales = lastMonthData?.MonthlyTotal ?? 0;
                decimal salesChangePercent = CalculatePercentageChange(lastMonthSales, currentMonthSales);

                // Determine change colors
                string salesChangeColor = salesChangePercent >= 0 ? "#28a745" : "#dc3545";
                string salesChangeBgColor = salesChangePercent >= 0 ? "#d4edda" : "#f8d7da";

                // Calculate bar heights (in pixels, max 180px)
                decimal maxSales = Math.Max(currentMonthSales, lastMonthSales);
                if (maxSales == 0) maxSales = 1; // Avoid division by zero

                int currentMonthBarHeight = (int)Math.Max(40, (currentMonthSales / maxSales) * 180);
                int lastMonthBarHeight = (int)Math.Max(40, (lastMonthSales / maxSales) * 180);

                // Load the HTML template
                string templatePath = Path.Combine(_env.WebRootPath, "EmailTemplates", "AdminMonthlyReportTemplate.html");
                string htmlTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

                // Generate products HTML
                string productsHtml = GenerateProductsHtml(topProducts.Take(10).ToList());

                // Replace all placeholders
                string htmlBody = htmlTemplate
                    .Replace("{{MonthName}}", currentMonthData.MonthName)
                    .Replace("{{Year}}", currentYear.ToString())
                    .Replace("{{CurrentMonthName}}", currentMonthData.MonthName)
                    .Replace("{{LastMonthName}}", lastMonthData?.MonthName ?? lastMonth.ToString("MMMM"))
                    .Replace("{{LastMonthYear}}", lastMonth.Year.ToString())
                    .Replace("{{CurrentMonthSales}}", FormatCurrency(currentMonthSales))
                    .Replace("{{LastMonthSales}}", FormatCurrency(lastMonthSales))
                    .Replace("{{SalesChangePercent}}", FormatPercentage(salesChangePercent))
                    .Replace("{{SalesChangeColor}}", salesChangeColor)
                    .Replace("{{SalesChangeBgColor}}", salesChangeBgColor)
                    .Replace("{{CurrentMonthBarHeight}}", currentMonthBarHeight.ToString())
                    .Replace("{{LastMonthBarHeight}}", lastMonthBarHeight.ToString())
                    .Replace("{{ProductsList}}", productsHtml)
                    .Replace("{{DashboardUrl}}", $"{Request.Scheme}://{Request.Host}/admin/dashboard");

                // Admin email
                string adminEmail = "manavevince@gmail.com";
                string adminName = "Gecko Gym Admin";
                string subject = $"Monthly Sales Report - {currentMonthData.MonthName} {currentYear}";

                // Send email
                bool sent = await _emailService.SendMonthlyReportEmailAsync(
                    adminEmail,
                    adminName,
                    subject,
                    htmlBody
                );

                if (sent)
                {
                    response.Data = new
                    {
                        Message = "Monthly report sent successfully",
                        SentTo = adminEmail,
                        Month = currentMonthData.MonthName,
                        Year = currentYear,
                        CurrentMonthSales = currentMonthSales,
                        LastMonthSales = lastMonthSales,
                        ChangePercent = salesChangePercent,
                        TopProductsCount = topProducts.Count
                    };
                    response.Success = true;
                    response.Message = "Monthly report sent successfully to admin.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Failed to send monthly report email.";
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
        /// Generate HTML for products list using table-based layout for email compatibility
        /// </summary>
        private string GenerateProductsHtml(List<MostOrderedProductsResponseModel> products)
        {
            if (products == null || !products.Any())
            {
                return @"<table role=""presentation"" width=""100%"" cellpadding=""20"" cellspacing=""0"" border=""0"">
                    <tr>
                        <td align=""center"" style=""color: #6c757d;"">No products data available.</td>
                    </tr>
                </table>";
            }

            var sb = new StringBuilder();
            long maxOrders = products.Max(p => p.OrderedCount);

            for (int i = 0; i < products.Count; i++)
            {
                var product = products[i];
                int rank = i + 1;
                string rankBgColor = rank == 1 ? "#FFD700" : rank == 2 ? "#C0C0C0" : rank == 3 ? "#CD7F32" : "#e9ecef";
                string rankTextColor = rank <= 3 ? "#ffffff" : "#495057";
                decimal progressPercent = maxOrders > 0 ? (decimal)product.OrderedCount / maxOrders * 100 : 0;

                sb.AppendLine($@"
            <table role=""presentation"" width=""100%"" cellpadding=""16"" cellspacing=""0"" border=""0"" bgcolor=""#ffffff"" style=""background-color: #ffffff; border-radius: 10px; margin-bottom: 12px; border: 2px solid transparent;"">
                <tr>
                    <td width=""36"" valign=""middle"">
                        <table role=""presentation"" width=""36"" height=""36"" cellpadding=""0"" cellspacing=""0"" border=""0"" bgcolor=""{rankBgColor}"" style=""background-color: {rankBgColor}; border-radius: 8px;"">
                            <tr>
                                <td align=""center"" valign=""middle"" style=""font-weight: 800; font-size: 16px; color: {rankTextColor};"">{rank}</td>
                            </tr>
                        </table>
                    </td>
                    <td width=""15""></td>
                    <td valign=""middle"">
                        <div style=""font-size: 15px; font-weight: 700; color: #1a1a1a; margin-bottom: 3px;"">{product.ProductName}</div>
                        <div style=""font-size: 12px; color: #6c757d; margin-bottom: 8px;"">{product.ProductDescription ?? "No description"}</div>
                        <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" bgcolor=""#e9ecef"" style=""background-color: #e9ecef; height: 6px; border-radius: 3px;"">
                            <tr>
                                <td width=""{progressPercent:F1}%"" bgcolor=""#ff6b35"" style=""background-color: #ff6b35; height: 6px; border-radius: 3px;""></td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                    <td width=""15""></td>
                    <td width=""80"" align=""right"" valign=""middle"">
                        <div style=""font-size: 20px; font-weight: 800; color: #ff6b35; margin-bottom: 2px;"">{product.OrderedCount}</div>
                        <div style=""font-size: 11px; color: #6c757d; text-transform: uppercase; letter-spacing: 0.5px;"">Orders</div>
                    </td>
                </tr>
            </table>");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Calculate percentage change between two values
        /// </summary>
        private decimal CalculatePercentageChange(decimal oldValue, decimal newValue)
        {
            if (oldValue == 0)
            {
                return newValue > 0 ? 100 : 0;
            }
            return Math.Round(((newValue - oldValue) / oldValue) * 100, 1);
        }

        /// <summary>
        /// Format currency value
        /// </summary>
        private string FormatCurrency(decimal value)
        {
            return value.ToString("N0");
        }

        /// <summary>
        /// Format percentage value with +/- sign
        /// </summary>
        private string FormatPercentage(decimal value)
        {
            return value >= 0 ? $"+{value:0.#}" : $"{value:0.#}";
        }

        #endregion
    }
}
