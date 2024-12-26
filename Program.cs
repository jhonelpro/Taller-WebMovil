using System.Text;
using api.src.Controller;
using api.src.Data;
using api.src.Helpers;
using api.src.Interfaces;
using api.src.Middleware;
using api.src.Models.User;
using api.src.Repositories;
using api.src.Service;
using CloudinaryDotNet;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.WithOrigins("http://localhost:4200")  // Tu frontend de Angular
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});

Env.Load();

// Establecer el nombre, la clave de API y la clave secreta de Cloudinary
string CloudinaryName = Environment.GetEnvironmentVariable("CLOUDINARY_NAME") ?? throw new ArgumentNullException("Cloudinary name cannot be null or empty.");
string CloudinaryApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY") ?? throw new ArgumentNullException("Cloudinary API key cannot be null or empty.");
string CloudinaryApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET") ?? throw new ArgumentNullException("Cloudinary API secret cannot be null or empty.");

    var CloudinaryAccount = new Account(
        CloudinaryName,
        CloudinaryApiKey,
        CloudinaryApiSecret
    );
var Cloudinary = new Cloudinary(CloudinaryAccount);
builder.Services.AddSingleton(Cloudinary);

builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IShoppingCart, ShoppingCartRepository>();
builder.Services.AddScoped<IShoppingCartItem, ShoppingCartItemRepository>();
builder.Services.AddScoped<IPurchase, PurchaseRepository>();
builder.Services.AddScoped<ISaleItem, SaleItemRepository>();
builder.Services.AddScoped<ITicket, TicketRepository>();
builder.Services.AddScoped<ICookieService, CookieService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentity<AppUser, IdentityRole>(
    opt => {
        opt.Password.RequireDigit = false;
        opt.Password.RequireLowercase = false;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequiredLength = 8;
    }
).AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme =
    opt.DefaultChallengeScheme =
    opt.DefaultForbidScheme = 
    opt.DefaultScheme =
    opt.DefaultSignInScheme =
    opt.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidateAudience = true,
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SIGNING_KEY") ?? throw new ArgumentNullException("Signing key cannot be null or empty."))),
    };
});

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

string connectionStringDB = Environment.GetEnvironmentVariable("DATA_BASE_URL") ?? "Data Source=app.db";
builder.Services.AddDbContext<ApplicationDBContext>(opt => opt.UseSqlite(connectionStringDB));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDBContext>();
    DataSeeder.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseMiddleware<BlacklistMiddleware>(); // Se agrega el middleware
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();