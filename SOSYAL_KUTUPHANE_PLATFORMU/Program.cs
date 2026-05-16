using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SOSYAL_KUTUPHANE_PLATFORMU.Data;
using SOSYAL_KUTUPHANE_PLATFORMU.Services;
using SOSYAL_KUTUPHANE_PLATFORMU.Middleware;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new Exception("JWT SecretKey tanýmlý deðil.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS Policy - GÜÇLENDIRILMIÞ
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
      "http://localhost:3000",
      "http://localhost:3001",
      "http://localhost:3002",  // ? Frontend port eklendi
      "http://localhost:5173",
      "http://localhost:4200",
    "http://localhost:8080"
 )
 .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithExposedHeaders("*"); // Tüm header'larý expose et
    });
});

// Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpClient<ITmdbService, TmdbService>();
builder.Services.AddHttpClient<IGoogleBooksService, GoogleBooksService>();

// Routing - Case Insensitive
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true; // URL'leri küçük harfe çevir
    options.LowercaseQueryStrings = false; // Query string'leri olduðu gibi býrak
});

builder.Services.AddControllers();

// Swagger Configuration with JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sosyal Kütüphane API",
        Version = "v1",
     Description = "Film ve kitap paylaþým platformu API"
    });

    // JWT Authorization için Swagger yapýlandýrmasý
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
     Description = "JWT Authorization header. Örnek: \"Bearer {token}\"",
    Name = "Authorization",
    In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
     Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
       Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Global Exception Handler (en üstte olmalý)
app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS - ÖNEMLÝ: Authentication'dan ÖNCE olmalý
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
