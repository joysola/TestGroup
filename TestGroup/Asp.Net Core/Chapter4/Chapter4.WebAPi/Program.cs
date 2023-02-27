using Chapter4.WebApi;
using Google.Protobuf.WellKnownTypes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureRepositoryManager(); // 
builder.Services.ConfigureServiceManager(); //
//builder.Services.ConfigureSqlContext(builder.Configuration); //
builder.Services.ConfigureSqlContext2(builder.Configuration); //
builder.Services.ConfigureLoggerService();
builder.Services.AddControllers().AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
