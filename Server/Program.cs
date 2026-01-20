using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server.Interfaces;
using Server.Repositories;
using Server.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();

var jwtKey = builder.Configuration["JWT_KEY"];

builder.Services.AddCors(options =>
{
    // options.AddDefaultPolicy(builder =>
    // {
    //     builder.WithOrigins("http://localhost:5173", "https://stocknotificator.tedweb.net")
    //         .AllowAnyHeader()
    //         .AllowAnyMethod();
    // });
    options.AddDefaultPolicy(policy =>
    {
        // 改用 SetIsOriginAllowed 比較彈性，可以避開 http/https 或斜線的問題
        policy.SetIsOriginAllowed(origin => 
            {
                // 開發環境允許 localhost
                if (origin == "http://localhost:5173") return true;
                
                // 生產環境允許你的網域
                if (origin.StartsWith("https://stocknotificator.tedweb.net")) return true;
                
                return false;
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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

builder.Services.Configure<FirebaseSetting>(options =>
{
    options.EmulatorHost = builder.Configuration["FIRESTORE_EMULATOR_HOST"];
    options.ProjectId = builder.Configuration["FIRESTORE_PROJECT_ID"];
});


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
                new string[] { }
            }
        });
});

builder.Services.AddHttpClient();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDataCenterService, DataCenterService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IStockNotificationService, StockNotificationService>();

builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IRepository, FirestoreRepository>();
builder.Services.AddSingleton<ICacheService, RedisService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsProduction())
{
    app.Use(async (context, next) =>
    {
        if (context.Request.Method == "OPTIONS")
        {
            context.Response.StatusCode = 204;
            return; 
        }
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await next();
            return;
        }

        var cfSecret = context.Request.Headers["HeaderFromCF"];
        if (cfSecret != app.Configuration["CLOUDFLARE_SECRET"])
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Direct access is prohibited.");
            return;
        }
        await next();
    });    
}


app.MapControllers();

app.Run();



public class FirebaseSetting
{
    public string EmulatorHost { get; set; }
    public string ProjectId { get; set; }
}