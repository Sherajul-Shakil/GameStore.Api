using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);

var app = builder.Build();

app.MapGamesEndpoints();
app.MapGenresEndpoints();
app.MapGet("/", () => "Hello, World!");

await app.MigrateDbAsync();

app.Run();


// dotnet run
// dotnet watch run
// dotnet clean
// dotnet restore
// dotnet build
// dotnet ef migrations add InitialCreate --output-dir Data\Migrations
// dotnet ef database update