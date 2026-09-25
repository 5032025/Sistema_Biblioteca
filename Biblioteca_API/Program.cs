using Aplicacion_API.Servicios;
using BookApplication.Services;
using Dominio_API.Interfaces;
using Infraestructura_API.Identidad;
using Infraestructura_API.Persistencia;
using Infraestructura_API.Persistencia.Repositorios;
using Infraestructura_API.Seguridad;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 1. Configuración de la conexión a la base de datos (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configuración de Identity para el manejo de usuarios y roles
builder.Services.AddIdentity<AppIdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 3. Registro de los repositorios en el contenedor de servicios
builder.Services.AddScoped<IAutor, AutorRepositorio>();
builder.Services.AddScoped<ICategoria, CategoriaRepositorio>();
builder.Services.AddScoped<ILibro, LibroRepositorio>();
builder.Services.AddScoped<IReserva, ReservaRepositorio>();
builder.Services.AddScoped<IUsuario, UsuarioRepositorio>();

// Registro de servicios de aplicación y autenticación
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AutorServicio>();
builder.Services.AddScoped<CategoriaServicio>();
builder.Services.AddScoped<LibroServicio>();
builder.Services.AddScoped<ReservaServicio>();
builder.Services.AddScoped<IJwt, JWTService>(); 
builder.Services.AddHttpContextAccessor();

// 4. Configuración de Autenticación JWT corregida
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
       
        ValidIssuer = builder.Configuration["JwtSettings:JwtIssuer"],
        ValidAudience = builder.Configuration["JwtSettings:JwtAudience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:JwtKey"] ?? "ClaveSuperSecretaDeRespaldo123!"))
    };
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Asegurar autenticación antes de la autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Inicialización de datos por defecto (Roles y Usuario Admin)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var usuarioRepositorio = services.GetRequiredService<IUsuario>();

        // 1. Crear los roles si no existen
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        string adminEmail = "admin@biblioteca.com";
        var usuarioAdmin = await usuarioRepositorio.GetUserByEmail(adminEmail); 

        if (usuarioAdmin == null)
        {
            var nuevoAdmin = new Dominio_API.Clases.Usuario
            {
                Email = adminEmail,
                Password = "Admin123*", // Limpia los espacios extra
                FirstName = "Administrador",
                LastName = "Sistema"
            };

            usuarioAdmin = await usuarioRepositorio.CreateUser(nuevoAdmin);
        }

        // Asegurar siempre que tenga el rol de Admin sin importar si ya existía
        if (usuarioAdmin != null)
        {
            await usuarioRepositorio.AddToRoleAsync(usuarioAdmin, "Admin");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar los roles y el usuario administrador.");
    }
}

app.Run();