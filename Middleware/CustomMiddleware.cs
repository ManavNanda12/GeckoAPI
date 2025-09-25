using GeckoAPI.Common;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

public class CustomMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtSettings _jwtSettings;
    private readonly EmailService _emailService;
    private readonly IWebHostEnvironment _env;

    public CustomMiddleware(RequestDelegate next, IOptions<JwtSettings> options, EmailService emailService, IWebHostEnvironment env)
    {
        _next = next;
        _jwtSettings = options.Value;
        _emailService = emailService;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    AttachUserToContext(context, token);
                }
                catch (SecurityTokenExpiredException ex)
                {
                    await SendErrorNotificationAsync(context, "TokenExpired", ex);
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token expired");
                    return;
                }
                catch (Exception ex)
                {
                    await SendErrorNotificationAsync(context, "InvalidToken", ex);
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid token");
                    return;
                }
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            // 🔥 Global catch: any exception in pipeline
            await SendErrorNotificationAsync(context, "UnhandledException", ex);
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Something went wrong (test global exception).");
        }
    }

    private async Task SendErrorNotificationAsync(HttpContext context, string errorType, Exception ex)
    {
        string filePath = Path.Combine(_env.WebRootPath, "EmailTemplates", "ExceptionTemplate.html");
        string htmlTemplate = await File.ReadAllTextAsync(filePath);
        string htmlBody = htmlTemplate
            .Replace("{{ErrorType}}", errorType)
            .Replace("{{ErrorMessage}}", ex.Message)
            .Replace("{{RequestPath}}", context.Request.Path)
            .Replace("{{TimeStamp}}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

        await _emailService.SendErrorEmailAsync($"JWT Error: {errorType}", htmlBody);
    }

    private void AttachUserToContext(HttpContext context, string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var userId = int.Parse(jwtToken.Subject);
        context.Items["UserId"] = userId;

        // Note: Removed the try-catch block here since exceptions are now handled in the main Invoke method
    }
}