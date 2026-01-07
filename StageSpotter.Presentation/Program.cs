using StageSpotter.Data.Interfaces;
using StageSpotter.Data.Repositories;
using StageSpotter.Business.Interfaces;
using StageSpotter.Business.Services;
using StageSpotter.Business.Builders; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IVacatureRepository, VacatureRepository>();
builder.Services.AddScoped<IBedrijfRepository, BedrijfRepository>();
builder.Services.AddScoped<IOpleidingsniveauRepository, OpleidingsniveauRepository>();
builder.Services.AddScoped<IStudierichtingRepository, StudierichtingRepository>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IAIService, GeminiService>();

builder.Services.AddScoped<ICVAnalyseService, CVAnalyseService>();


builder.Services.AddScoped<IVacatureService>(provider => 
{
    var vacatureRepo = provider.GetRequiredService<IVacatureRepository>();
    var bedrijfRepo = provider.GetRequiredService<IBedrijfRepository>();
    var opleidingRepo = provider.GetRequiredService<IOpleidingsniveauRepository>();
    var studieRepo = provider.GetRequiredService<IStudierichtingRepository>();

    var serviceBuilder = new VacatureServiceBuilder
    {
        VacatureRepo = vacatureRepo,
        BedrijfRepo = bedrijfRepo,
        OpleidingRepo = opleidingRepo,
        StudieRepo = studieRepo
    };

    return serviceBuilder.Build();
});


var app = builder.Build();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

StageSpotter.Data.DatabaseInitializer.Initialize(connectionString);
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
