using IdealDotnetApi.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;

var builder = WebApplication.CreateBuilder(args);
// Adding Logging
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
// For Logging
builder.Services.ConfigureLoggerService();
// For Controllers
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts(); // for Strict-Transport-Security headers
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });
app.UseCors("CorsPolicy");

app.UseAuthorization();
// Middleware Components
app.Use(async (context, next) =>
{
    Console.WriteLine($"Execution Order: 1. Logic before executing the next delegate in the Use method");
    await next.Invoke();
    Console.WriteLine($"Execution Order: 3. Logic after executing the next delegate in the Use method");
});

// https://localhost:5001/usingmapbranch 
app.Map("/usingmapbranch",
    builder =>
    {
        builder.Use(async (context, next) =>
        {
            Console.WriteLine("Map branch logic in the Use method before the next delegate");
            await next.Invoke();
            Console.WriteLine("Map branch logic in the Use method after the next delegate");
        });
        builder.Run(async context =>
        {
            Console.WriteLine($"Map branch response to the client in the Run method");
            await context.Response.WriteAsync("Hello from the map branch.");
        });
    });
// https://localhost:5001?testquerystring=test:
app.MapWhen(context => context.Request.Query.ContainsKey("testquerystring"),
    builder =>
    {
        builder.Run(async context =>
        {
            await context.Response.WriteAsync("Hello from the MapWhen branch.");
        });
    });


app.Run(async context =>
{
    Console.WriteLine($"Execution Order: 2. Writing the response to the client in the Run method");
    await context.Response.WriteAsync("Hello from the middleware component.");
});

// any middleware component that we add after the Map method in the pipeline won’t be executed
app.MapControllers();

app.Run();