using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PluralSight_CityAPI;
using PluralSight_CityAPI.DbContexts;
using PluralSight_CityAPI.Services;
using Serilog;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;

// Create the 3rd-part logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityInfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Change logger to SeriLog
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
})
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();

// Add authentication JWT Token services
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretInfoKey"]))
        };
    });

// Add authorisation policy
// Use by adding an attribute: [Authorize(Policy = <Policy_Name>)]
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustFromHCM", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "HCM");
    });
});




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAct =>
{
    var xmlfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlpath = Path.Combine(AppContext.BaseDirectory, xmlfile) ;

    setupAct.IncludeXmlComments(xmlpath);

    setupAct.AddSecurityDefinition("CityApiBearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Invalid bearer method"
    });

    setupAct.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
             new OpenApiSecurityScheme
             {
                 Reference = new OpenApiReference
                 {
                     Type = ReferenceType.SecurityScheme,
                     Id = "CityApiBearer"
                 }
             }, new List<string>()
        }
    });
});

// Add Singleton (1 service for all requests) to inject services for retrieving content type
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

// Add Transient (1 new service for each call) to inject new mail service
#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

// Add CitiesDataStore as a Service
builder.Services.AddSingleton<CitiesDataStore>();

// Add context of database
builder.Services.AddDbContext<CityInfoContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CityInfo")));

// Add scoped repository service
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

// Add AutoMapper 3rd-party services
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add Versioning
builder.Services.AddApiVersioning(setupAct =>
{
    setupAct.AssumeDefaultVersionWhenUnspecified = true;
    setupAct.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    setupAct.ReportApiVersions = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
