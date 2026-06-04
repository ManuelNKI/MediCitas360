using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURACIÓN DE PUERTOS DINÁMICOS
var port = args.Contains("--port") ? args[Array.IndexOf(args, "--port") + 1] : "5001";
builder.WebHost.UseUrls($"http://localhost:{port}");

// 2. AGREGAR SERVICIOS AL CONTENEDOR DE DEPENDENCIAS
builder.Services.AddControllers();

// Inyección de tus cadenas de conexión y repositorios distribuidos (Supabase + Local)
builder.Services.AddInfrastructure(builder.Configuration);

// Soporte nativo de OpenAPI de .NET 9/10
builder.Services.AddOpenApi();

var app = builder.Build();

// 3. CONFIGURACIÓN DEL PIPELINE DE PETICIONES HTTP (MIDDLEWARE)
if (app.Environment.IsDevelopment())
{
    // Mapea el endpoint del documento OpenAPI (Json de metadatos)
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

Console.WriteLine($"\n=========================================================");
Console.WriteLine($"🚀 MEDICITAS 360 - API ACTIVA EN: http://localhost:{port}");
Console.WriteLine($"=========================================================\n");

app.Run();