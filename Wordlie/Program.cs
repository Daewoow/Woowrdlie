using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Wordlie.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR().AddHubOptions<GameHub>(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(380);
    options.HandshakeTimeout = TimeSpan.FromSeconds(380);
    options.KeepAliveInterval = TimeSpan.FromSeconds(380);
    options.EnableDetailedErrors = true;
    options.MaximumParallelInvocationsPerClient = Environment.ProcessorCount - 1;
});

var app = builder.Build();

app.MapHub<GameHub>("/game", options => {
    options.Transports = HttpTransportType.WebSockets;
});
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/mainpage", context =>
{
    context.Response.Redirect("src/pages/login.html");
    return Task.CompletedTask;
});

app.Run();
