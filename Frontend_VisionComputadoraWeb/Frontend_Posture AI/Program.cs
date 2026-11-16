using Microsoft.ML.OnnxRuntime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// INYECCION DEL MODELO IA (Singleton)
builder.Services.AddSingleton<InferenceSession>(sp =>
{
    // Asegúrate de que tu compañero de Datos te pase el archivo o crea uno temporal para que no falle.
    return new InferenceSession("MLModels/postura_model.onnx");
});

var app = builder.Build();

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

// Habilitar archivos estáticos explícitamente y configurar el entorno
app.UseStaticFiles();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
