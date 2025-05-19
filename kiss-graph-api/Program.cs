var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(Neo4j.Driver.GraphDatabase.Driver("bolt://localhost:7687", Neo4j.Driver.AuthTokens.Basic("neo4j", "nibla9687")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();                         // This exposes the generated Swagger JSON endpoint (e.g., /swagger/v1/swagger.json)
    app.UseSwaggerUI();                       // This serves the Swagger UI interactive documentation
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
