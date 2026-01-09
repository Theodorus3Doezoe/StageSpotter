using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StageSpotter.Data.Interfaces;
using StageSpotter.Data.Repositories;
using StageSpotter.Business.Interfaces;
using StageSpotter.Business.Services;
using StageSpotter.Business.Builders;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
});

var connectionStringForServices = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=stagespotter.db";

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


builder.Services.AddScoped<IUserRepository>(provider => new UserRepository(connectionStringForServices));
builder.Services.AddScoped<IAuthService>(provider =>
{
    var userRepo = provider.GetRequiredService<IUserRepository>();
    var bedrijfRepo = provider.GetRequiredService<IBedrijfRepository>();
    var signingKey = provider.GetRequiredService<Microsoft.IdentityModel.Tokens.SymmetricSecurityKey>();
    return new AuthService(userRepo, bedrijfRepo, signingKey);
});
builder.Services.AddScoped<ISavedAnalysisRepository>(provider => new SavedAnalysisRepository(connectionStringForServices));
builder.Services.AddScoped<ISavedVacatureRepository>(provider => new SavedVacatureRepository(connectionStringForServices));
builder.Services.AddScoped<ISavedItemService, SavedItemService>();
builder.Services.AddScoped<IReviewRepository>(provider => new StageSpotter.Data.Repositories.ReviewRepository(connectionStringForServices));
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IQuizRepository>(provider => new StageSpotter.Data.Repositories.QuizRepository(connectionStringForServices));
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IMotivationLetterService, MotivationLetterService>();
builder.Services.AddScoped<ISavedMotivationLetterRepository>(provider => new StageSpotter.Data.Repositories.SavedMotivationLetterRepository(connectionStringForServices));

var jwtKey = builder.Configuration["Jwt:Key"] ?? "very_secret_key_12345";
var derivedKeyBytes = Encoding.UTF8.GetBytes(jwtKey);
var symmetricKey = new SymmetricSecurityKey(derivedKeyBytes);
builder.Services.AddSingleton(symmetricKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return System.Threading.Tasks.Task.CompletedTask;
        }
    };
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = symmetricKey,
        ClockSkew = System.TimeSpan.Zero
    };
});

var app = builder.Build();

var connectionString = connectionStringForServices;

StageSpotter.Data.DatabaseInitializer.Initialize(connectionString);
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 401 && !context.Request.Path.StartsWithSegments("/Auth"))
    {
        context.Response.Redirect("/Auth/Login");
    }
});

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
