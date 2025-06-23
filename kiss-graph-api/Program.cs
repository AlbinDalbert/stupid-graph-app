using kiss_graph_api.Middleware;
using kiss_graph_api.Repositories.Interfaces;
using kiss_graph_api.Repositories.Neo4j;
using kiss_graph_api.Services.Implementations;
using kiss_graph_api.Services.Interfaces;
using Neo4j.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var neo4jUri = Environment.GetEnvironmentVariable("NEO4J_URI");       // Should be "bolt://graphdb:7687"
var neo4jUser = Environment.GetEnvironmentVariable("NEO4J_USER");     // Should be "neo4j"
var neo4jPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD"); // Should be "mypass" (or your chosen password)

// Simple check for null/empty to help debugging if environment variables aren't resolving
if (string.IsNullOrEmpty(neo4jUri) || string.IsNullOrEmpty(neo4jUser) || string.IsNullOrEmpty(neo4jPassword))
{
    // You might want to log this more formally
    Console.WriteLine("CRITICAL: Neo4j connection details are missing from environment variables.");
    // Optionally, throw an exception or handle as appropriate for your app's startup
}
else
{
    Console.WriteLine($"Neo4j URI: {neo4jUri}"); // Good for debugging startup
}

// Register Neo4j IDriver as a singleton
builder.Services.AddSingleton<IDriver>(GraphDatabase.Driver(neo4jUri, AuthTokens.Basic(neo4jUser, neo4jPassword)));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowViteDevServer",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod();
        });
});

//add services
// --- REGISTER YOUR NEW SERVICES AND REPOSITORIES ---
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IFranchiseService, FranchiseService>();
//add repos
builder.Services.AddScoped<IMovieRepository, Neo4JMovieRepository>();
builder.Services.AddScoped<IPersonRepository, Neo4JPersonRepository>();
builder.Services.AddScoped<ICharacterRepository, Neo4JCharacterRepository>();
builder.Services.AddScoped<IActedInRepository, Neo4JActedInRepository>();
builder.Services.AddScoped<IPortrayedRepository, Neo4JPortrayedRepository>();
builder.Services.AddScoped<IAppearsInRepository, Neo4JAppearsInRepository>();
builder.Services.AddScoped<IGenreRepository, Neo4JGenreRepository>();
builder.Services.AddScoped<IFranchiseRepository, Neo4JFranchiseRepository>();
builder.Services.AddScoped<ICWInFranchiseRepository, Neo4JCWInFranchiseRepository>();
builder.Services.AddScoped<ICInFranchiseRepository, Neo4JCInFranchiseRepository>();
builder.Services.AddScoped<IInGenreRepository, Neo4JInGenreRepository>();

var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();                         // This exposes the generated Swagger JSON endpoint (e.g., /swagger/v1/swagger.json)
    app.UseSwaggerUI();                       // This serves the Swagger UI interactive documentation
}

app.UseStaticFiles();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowViteDevServer");

app.Run();
