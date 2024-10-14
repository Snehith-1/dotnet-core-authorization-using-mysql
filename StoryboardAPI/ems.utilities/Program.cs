using ems.utilities.Functions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<dbconn>();
builder.Services.AddSingleton<cmnfunctions>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
