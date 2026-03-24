using DemoWebAPI.model.Models;
using DemoWebAPI.Repository;
using DemoWebAPI.Service;
using FirebaseAdmin;
using GeckoAPI.Common;
using GeckoAPI.Model.models;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(int.Parse(port));
});

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
        };
    });

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"),
        new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));


builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Gecko API", Version = "v1" });

    // 🔑 Add JWT bearer security definition
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid JWT token."
    });

    // 🔒 Add security requirement so endpoints need auth by default
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// Database Connection
builder.Services.Configure<DbConfig>(
    builder.Configuration.GetSection("ConnectionStrings")
);
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHangfireServer();
builder.Services.AddHttpClient();
builder.Services.AddTransient<EmailJob>();
builder.Services.AddScoped<PushNotificationJob>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
});



//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//    ConnectionMultiplexer.Connect(
//        builder.Configuration["Redis:ConnectionString"]
//    ));


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    // Enable Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
});

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("geckocustomerportal-firebase-keys.json")
});


// Register recurring job:
RecurringJob.AddOrUpdate<EmailJob>(
    "send-welcome-emails",
    job => job.SendWelcomeEmailsViaApi(),
    "*/15 * * * *");
//RecurringJob.AddOrUpdate<EmailJob>(
//    "clear-cache-memory",
//    job=> job.ClearAllCache(),
//    "0 0 * * *"
//    );
RecurringJob.AddOrUpdate<EmailJob>(
    "send-monthly-reports",
    job => job.SendMonthlySalesReportViaApi(),
     "0 2 1 * *");

RecurringJob.AddOrUpdate<PushNotificationJob>(
    "abandoned-cart-notifications",
    job => job.SendAbandonedCartNotifications(),
    "*/5 * * * *"
);


app.UseCors("AllowAll");
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CustomMiddleware>();
app.MapControllers();

app.Run();