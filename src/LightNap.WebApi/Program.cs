using LightNap.Core.Data;
using LightNap.Core.Extensions;
using LightNap.Core.Identity;
using LightNap.Migrations.SqlServer.Extensions;
using LightNap.WebApi.Configuration;
using LightNap.WebApi.Extensions;
using LightNap.WebApi.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection("SiteSettings"));
builder.Services.Configure<List<AdministratorConfiguration>>(builder.Configuration.GetSection("Administrators"));

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions((options) =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Select a DB provider. Ensure you reference the appropriate library and update appsettings.config if necessary.
builder.Services.AddLightNapSqlServer(builder.Configuration);

// Select an email provider. Ensure you reference the appropriate library and update appsettings.config if necessary.
builder.Services.AddLogToConsoleEmailer();
//builder.Services.AddSmtpEmailer();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseCors(policy =>
    policy
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

var logger = services.GetService<ILogger<Program>>() ?? throw new Exception($"Logging is not configured, so there may be deeper configuration issues");

try
{
    var context = services.GetRequiredService<ApplicationDbContext>();
    var siteSettings = services.GetRequiredService<IOptions<SiteSettings>>();
    if (siteSettings.Value.AutomaticallyApplyEfMigrations)
    {
        await context.Database.MigrateAsync();
    }

    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
    await Seeder.SeedRoles(roleManager, logger);

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var administratorSettings = services.GetRequiredService<IOptions<List<AdministratorConfiguration>>>();
    await Seeder.SeedAdministrators(userManager, roleManager, administratorSettings, logger);
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred during migration and/or seeding");
    throw;
}

app.Run();
