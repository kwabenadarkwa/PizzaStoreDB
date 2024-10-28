using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(static c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "PizzaStore API",
            Description = "Making the Pizzas you love",
            Version = "v1",
        }
    );
});
WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI(static c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
    });
}

app.MapGet("/", static () => "Hello World!");

app.Run();
