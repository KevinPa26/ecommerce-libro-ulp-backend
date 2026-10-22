using ecommerce_libro_ulp_backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
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

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)
            ),

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),

            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            NameClaimType = System.Security.Claims.ClaimTypes.Name
        };
    });
// builder.Services.AddHttpContextAccessor();//Para recuperar el user entre otros datos
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ILibroRepository, LibroRepository>();
builder.Services.AddScoped<ICarritoRepository, CarritoRepository>();
builder.Services.AddScoped<IFavoritoRepository, FavoritoRepository>();

// var connectionString = builder.Configuration
//     .GetConnectionString("DefaultConnection");

// if (string.IsNullOrWhiteSpace(connectionString))
// {
//     throw new InvalidOperationException(
//         "No se encontró la cadena de conexión DefaultConnection.");
// }

//para conectar con url en appsetting
//builder.Configuration["ConnectionStrings:MySqlConnectionString"]

builder.Services.AddDbContext<AppDbContext>(options =>
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
            .GetRequiredService<AppDbContext>();

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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
