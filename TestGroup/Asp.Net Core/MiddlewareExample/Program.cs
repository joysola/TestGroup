var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

//app.Use(async (context, next) =>
//{
//    Console.WriteLine($"Logic before executing the next delegate in the Use method");
//    await next.Invoke();
//    Console.WriteLine($"Logic after executing the next delegate in the Use method");
//}); //

//app.Run(async context =>
//{
//    Console.WriteLine($"Writing the response to the client in the Run method");
//    await context.Response.WriteAsync("Hello from the middleware component.");
//}); //

app.Use(async (context, next) =>
{
    await context.Response.WriteAsync("Hello from the middleware component.");
    await next.Invoke();
    Console.WriteLine($"Logic after executing the next delegate in the Use method");
});
app.Run(async context =>
{
    Console.WriteLine($"Writing the response to the client in the Run method");
    context.Response.StatusCode = 200;
    await context.Response.WriteAsync("Hello from the middleware component.");
});

app.MapControllers();

app.Run();
