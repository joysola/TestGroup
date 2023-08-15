using Chapter9.WebApi.Extensions;
using Chapter9.WebApi;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Formatters;
using CompanyEmployees.Presentation;
using Service.DataShaping;
using Shared.DataTransferObjects;
using CompanyEmployees.Presentation.ActionFilters;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureRepositoryManager(); // 
builder.Services.ConfigureServiceManager(); //
//builder.Services.ConfigureSqlContext(builder.Configuration); //
builder.Services.ConfigureSqlContext2(builder.Configuration); //
builder.Services.ConfigureLoggerService();
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();

// 去除apicontrollr的自动验证参数（参数）
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<ValidateMediaTypeAttribute>();


builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true; // Accept text/xml、text/json
    config.ReturnHttpNotAcceptable = true; // 406
    config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
    config.CacheProfiles.Add("120SecondsDuration", new CacheProfile
    {
        Duration = 120
    });
})
 .AddXmlDataContractSerializerFormatters()
 .AddCustomCSVFormatter()
 .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);

builder.Services.AddCustomMediaTypes();

builder.Services.AddAutoMapper(typeof(Program)); //

builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureVersioning();

builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();
//builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);
if (app.Environment.IsProduction())
    app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});


app.UseIpRateLimiting();

app.UseCors("CorsPolicy");

app.UseResponseCaching();
app.UseHttpCacheHeaders();

app.UseAuthorization();

app.MapControllers();

app.Run();


NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
    new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
    .Services.BuildServiceProvider()
    .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
    .OfType<NewtonsoftJsonPatchInputFormatter>().First();