using Frontedn_Posture_AI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML.OnnxRuntime;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

// INYECCION DEL MODELO IA (Singleton)
builder.Services.AddSingleton<InferenceSession>(sp =>
{
    // El archivo MLP
    return new InferenceSession("MLModels/postura_model.onnx");
});

// BASE DE DATOS (PostgreSQL + Entity Framework Core)
// La cadena se lee de ConnectionStrings:DefaultConnection. Nunca se escribe en
// appsettings.json: en local se usa "dotnet user-secrets" o una variable de
// entorno, y en Cloud Run la variable ConnectionStrings__DefaultConnection.
var cadenaConexion = builder.Configuration.GetConnectionString("DefaultConnection");
var baseDatosHabilitada = !string.IsNullOrWhiteSpace(cadenaConexion);

builder.Services.AddSingleton(new EstadoBaseDatos(baseDatosHabilitada));
builder.Services.AddSingleton<ControlRegistros>();
builder.Services.AddDbContext<AppDbContext>(options =>
    // Sin cadena configurada se usa un valor local solo para que EF pueda
    // generar migraciones; la app no intentará conectarse.
    options.UseNpgsql(baseDatosHabilitada ? cadenaConexion : "Host=localhost;Database=postureai"));

var app = builder.Build();

// Aplicar migraciones pendientes al iniciar (crea las tablas y los datos semilla)
if (baseDatosHabilitada)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        app.Logger.LogInformation("Base de datos conectada y migraciones aplicadas.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "No se pudo conectar o migrar la base de datos. La IA seguirá funcionando sin guardar historial.");
    }
}
else
{
    app.Logger.LogWarning("ConnectionStrings:DefaultConnection no está configurada. El historial no se guardará.");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();


app.UseStaticFiles();

app.MapStaticAssets();

// Verificación rápida de la base de datos (útil durante el pitch)
app.MapGet("/health/db", async (AppDbContext db, EstadoBaseDatos estado) =>
{
    if (!estado.Habilitada)
    {
        return Results.Json(new { baseDatos = "no configurada" }, statusCode: 503);
    }
    try
    {
        var conectada = await db.Database.CanConnectAsync();
        if (!conectada)
        {
            return Results.Json(new { baseDatos = "sin conexión" }, statusCode: 503);
        }
        return Results.Ok(new
        {
            baseDatos = "conectada",
            clinicas = await db.Clinicas.CountAsync(),
            pacientes = await db.Pacientes.CountAsync(),
            ejercicios = await db.Ejercicios.CountAsync(),
            evaluaciones = await db.EvaluacionesPosturales.CountAsync()
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new { baseDatos = "error", detalle = ex.Message }, statusCode: 503);
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
