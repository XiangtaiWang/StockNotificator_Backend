using System.Text;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server.Controllers;
using Server.Interfaces;
using Server.Repositories;
using Server.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();

var jwtKey = builder.Configuration["JWT_KEY"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, 
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

var redisUrl = builder.Configuration["REDIS_HOST"];
var redisToken = builder.Configuration["REDIS_TOKEN"] ?? "";

var connectionString = string.IsNullOrEmpty(redisToken) 
    ? redisUrl 
    : $"{redisUrl}:6379,password={redisToken},ssl=true,abortConnect=false";


var redisConnection = ConnectionMultiplexer.Connect(connectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);

builder.Services.Configure<FirebaseSetting>(options => {
    options.EmulatorHost = builder.Configuration["FIRESTORE_EMULATOR_HOST"];
    options.ProjectId = builder.Configuration["FIRESTORE_PROJECT_ID"];
});

// builder.Services.AddHangfire(configuration => configuration
//     .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
//     .UseSimpleAssemblyNameTypeSerializer()
//     .UseRecommendedSerializerSettings()
//     .UseRedisStorage(redisConnection, new RedisStorageOptions
//     {
//         Prefix = "hangfire:",
//     }));
// builder.Services.AddHangfireServer();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", 
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
});

builder.Services.AddHttpClient();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDataCenterService, DataCenterService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IStockNotificationService, StockNotificationService>();

builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IRepository, FirestoreRepository>();
builder.Services.AddSingleton<ICacheService, RedisService>();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHangfireDashboard();
app.UseHttpsRedirection();
app.MapControllers();
// app.UseAuthorization();
// app.UseAuthentication();
// RegisterRecurringJobs();
app.Run();

// void RegisterRecurringJobs()
// {
//     
//     RecurringJob.AddOrUpdate<JobController>(
//         "Minutely-Fetch-Stock-Info",
//         service => service.SystemMinutelyJob() ,
//         "* 9-21 * * 1-5",
//         new RecurringJobOptions
//         {
//             TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei")
//         }
//     );
//
//     
// }

public class FirebaseSetting
{
    public string EmulatorHost{ get; set; }
    public string ProjectId{ get; set; }
}