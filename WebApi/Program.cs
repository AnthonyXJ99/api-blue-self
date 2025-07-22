using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Reflection;
using BlueSelfCheckout.Data;
using Microsoft.Extensions.FileProviders; // <-- IMPORTANTE para servir archivos estáticos

var builder = WebApplication.CreateBuilder(args);

// **PASO 1: Define una política CORS**
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; // Un nombre para tu política

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:7115", // Permite tu frontend de desarrollo
                                              "http://localhost:4200", // ← AGREGADO: Tu Angular app
                                              "http://192.168.18.43:5023", // Si accedes al frontend por IP
                                              "http://192.168.1.10:8080",// Ejemplo: otra IP y puerto
                                              "http://192.168.18.239:4200")
                                 .AllowAnyHeader() // Permite cualquier cabecera en la solicitud
                                 .AllowAnyMethod(); // Permite cualquier método HTTP (GET, POST, PUT, DELETE, etc.)
                                                    // .AllowCredentials(); // Agrega esto si tu frontend envía cookies o credenciales
                      });
});

// --- FIN DE LA POLÍTICA CORS ---
builder.Services.AddAutoMapper(typeof(Program).Assembly); // O typeof(MappingProfile).Assembly;


// Configuración de la cultura en la aplicación
var supportedCultures = new[] { "en-US", "es-ES" };
var cultureInfo = new CultureInfo("en-US");

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebApiContext") ?? throw new InvalidOperationException("Cadena de conexión 'WebApiContext' no encontrado.")));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Configuración básica de Swagger
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "1.0",
        Title = "Blue Self Checkout",
        Description = "Web API Blue Self Checkout",
        Contact = new OpenApiContact
        {
            Name = "Raul Lara",
            Email = "raul.lara@bartech.com"
        }
    });

    // Fix for CS0117: 'Assembly' no contiene una definición para 'GetExecutingExecutingAssembly'
    // The correct method name is 'GetExecutingAssembly'. Update the code as follows:

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(cultureInfo);
    options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.SupportedUICultures = options.SupportedCultures;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// **PASO 2: APLICAR LA POLÍTICA CORS - ¡ESTO ES LO QUE FALTABA!**
app.UseCors(MyAllowSpecificOrigins); // ← AGREGADO: Aplica la política CORS

// --- CONFIGURACIÓN PARA SERVIR LA CARPETA IMAGES ---
var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath); // Opcional: crea la carpeta si no existe
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
    RequestPath = "/images" // Acceso: http://localhost:xxxx/images/tuimagen.jpg
});
// --- FIN DE CONFIGURACIÓN IMAGES ---

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
    db.Database.Migrate(); // Aplica migraciones pendientes y crea la BD si no existe
}

app.Run();