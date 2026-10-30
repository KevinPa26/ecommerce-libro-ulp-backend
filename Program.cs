using ecommerce_libro_ulp_backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Autenticación y validación de JWT.
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException(
        "Falta configurar Jwt:Secret en la configuración segura.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "Falta configurar Jwt:Issuer.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "Falta configurar Jwt:Audience.");

// Autenticación por cookies para MVC y JWT para API.
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;

        options.DefaultSignInScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";

        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy =
            CookieSecurePolicy.Always;

        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        builder.Configuration["Jwt:Secret"]
                        ?? throw new InvalidOperationException(
                            "Falta configurar Jwt:Secret.")
                    )
                ),

                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],

                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),

                RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                NameClaimType = System.Security.Claims.ClaimTypes.Name
            };
    });

builder.Services.AddAuthorization();

// builder.Services.AddHttpContextAccessor();//Para recuperar el user entre otros datos
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ILibroRepository, LibroRepository>();
builder.Services.AddScoped<ICarritoRepository, CarritoRepository>();
builder.Services.AddScoped<IFavoritoRepository, FavoritoRepository>();
builder.Services.AddScoped<IImagenLibroRepository, ImagenLibroRepository>();
builder.Services.AddScoped<ICompraRepository, CompraRepository>();

// var connectionString = builder.Configuration
//     .GetConnectionString("DefaultConnection");

// if (string.IsNullOrWhiteSpace(connectionString))
// {
//     throw new InvalidOperationException(
//         "No se encontró la cadena de conexión DefaultConnection.");
// }

//para conectar con url en appsetting
//builder.Configuration["ConnectionStrings:MySqlConnectionString"]

builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(
        builder.Configuration["ConnectionStrings:MySqlConnectionString"],
        new MySqlServerVersion(new Version(8, 0, 36))
    )
);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    try
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<DataContext>();

        var conectado = await db.Database.CanConnectAsync();

        Console.WriteLine(
            conectado
                ? "OK: conexión con base de datos establecida."
                : "ERROR: no se pudo conectar con la base de datos."
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR DE CONEXIÓN: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
    name: "mi-carrito",
    pattern: "MiCarrito",
    defaults: new
    {
        controller = "Carritos",
        action = "Index"
    })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Libros}/{action=Index}/{id?}")
    .WithStaticAssets();



app.Run();
