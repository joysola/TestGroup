using Chapter7.WebApi.Extensions;
using Chapter7.WebApi;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureRepositoryManager(); // 
builder.Services.ConfigureServiceManager(); //
//builder.Services.ConfigureSqlContext(builder.Configuration); //
builder.Services.ConfigureSqlContext2(builder.Configuration); //
builder.Services.ConfigureLoggerService();
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));



builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true; // Accept text/xml¡¢text/json
    config.ReturnHttpNotAcceptable = true; // 406
})
    .AddXmlDataContractSerializerFormatters()
    .AddCustomCSVFormatter()
.AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);

builder.Services.AddAutoMapper(typeof(Program)); //

builder.Services.AddControllers();

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
app.UseCors("CorsPolicy");



app.UseAuthorization();

app.MapControllers();

app.Run();


