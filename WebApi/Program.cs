using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Reflection;
using BlueSelfCheckout.Data;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Configuración específica para IIS
builder.WebHost.UseIISIntegration();


// CORS configurado para IIS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .WithExposedHeaders("x-pagination");
        });
});


builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Configuración de la cultura
var supportedCultures = new[] { "en-US", "es-ES" };
var cultureInfo = new CultureInfo("en-US");

// Configuración de Entity Framework con retry para IIS
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Cadena de conexión 'WebApiContext' no encontrado."),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
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

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // Verificar si el archivo XML existe antes de incluirlo
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(cultureInfo);
    options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.SupportedUICultures = options.SupportedCultures;
});

// Configuración de logging para IIS
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

var app = builder.Build();

// Configuración del pipeline para IIS
if (app.Environment.IsDevelopment() )
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    // Para producción en IIS
    app.UseExceptionHandler("/Error");
    app.UseHsts();

    // Swagger también en producción (opcional, puedes comentar estas líneas)
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blue Self Checkout API v1");
        c.RoutePrefix = "swagger"; // Acceso: http://tuservidor/swagger
    });
}



// **CONFIGURACIÓN PARA SERVIR LA CARPETA IMAGES OPTIMIZADA PARA IIS**
var contentRoot = app.Environment.ContentRootPath;
var imagesPath = Path.Combine(contentRoot, "Images");

if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
    RequestPath = "/images"
});

// ⚠️ IMPORTANTE: UseRouting DESPUÉS de UseCors
app.UseRouting();

// ⚠️ COMENTADO: UseHttpsRedirection puede causar problemas con CORS preflight
// Si necesitas HTTPS, úsalo DESPUÉS de CORS y configúralo correctamente
// app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

// Migración de base de datos con mejor manejo de errores
// En tu Program.cs ya tienes esto, pero agrega más detalles:
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        // AGREGAR ESTA INFORMACIÓN PARA DEBUG
        var connectionString = db.Database.GetConnectionString();
        var serverName = db.Database.GetDbConnection().DataSource;
        var databaseName = db.Database.GetDbConnection().Database;

        logger.LogInformation($"Servidor: {serverName}");
        logger.LogInformation($"Base de datos: {databaseName}");
        logger.LogInformation($"Connection String: {connectionString}");

        logger.LogInformation("Aplicando migraciones de base de datos...");
        db.Database.Migrate();
        logger.LogInformation("Migraciones aplicadas exitosamente.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error al aplicar migraciones de base de datos");
}

app.Run();