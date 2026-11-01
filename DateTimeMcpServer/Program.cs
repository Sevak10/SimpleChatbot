var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithTools<DateTimeTools>();

var app = builder.Build();

app.MapMcp("/mcp");

app.Run();