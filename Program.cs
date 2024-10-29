using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PizzaStoreDB.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
string connectionString =
    builder.Configuration.GetConnectionString("Pizzas") ?? "Data Source=Pizzas.db";

builder.Services.AddEndpointsApiExplorer();

// builder.Services.AddDbContext<PizzaDb>(static options => options.UseInMemoryDatabase("items"));
builder.Services.AddSqlite<PizzaDb>(connectionString);
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
app.MapGet("/pizzas", static async (PizzaDb db) => await db.Pizzas.ToListAsync());
// 
app.MapPost(
    "/pizza",
    static async (PizzaDb db, Pizza pizza) =>
    {
        _ = await db.Pizzas.AddAsync(pizza);
        _ = await db.SaveChangesAsync();
        return Results.Created($"/pizza/{pizza.Id}", pizza);
    }
);
app.MapGet("/pizza/{id}", static async (PizzaDb db, int id) => await db.Pizzas.FindAsync(id));
app.MapPut(
    "/pizza/{id}",
    static async (PizzaDb db, Pizza updatepizza, int id) =>
    {
        Pizza? pizza = await db.Pizzas.FindAsync(id);
        if (pizza is null)
        {
            return Results.NotFound();
        }

        pizza.Name = updatepizza.Name;
        pizza.Description = updatepizza.Description;
        _ = await db.SaveChangesAsync();
        return Results.NoContent();
    }
);
app.MapDelete(
    "/pizza/{id}",
    static async (PizzaDb db, int id) =>
    {
        Pizza? pizza = await db.Pizzas.FindAsync(id);
        if (pizza is null)
        {
            return Results.NotFound();
        }
        _ = db.Pizzas.Remove(pizza);
        _ = await db.SaveChangesAsync();
        return Results.Ok();
    }
);
app.Run();
