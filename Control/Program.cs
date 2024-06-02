using System.Net.WebSockets;
using Control.Components;
using Control.Services;
using Control.Setup;
using Microsoft.AspNetCore.WebSockets;
using SCHIZO.RemoteControl.Constants;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IServiceCollection services = builder.Services;
builder.Logging.AddConsole();

services.AddWebSockets(op => op.KeepAliveInterval = TimeSpan.FromSeconds(10));

services.AddSingleton<IsDeveloperService>();
services.AddSingleton<GameConnectionManager>();
services.AddScoped<UserService>();

builder.AddAuth();
services.AddHttpContextAccessor();
services.AddHttpLogging(_ => { });

services.AddScoped<GameConnectionService>();

services.AddDistributedMemoryCache();
services.AddSession();
services.AddRateLimiter(RateLimiting.ConfigureRateLimiting);

services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseHttpLogging();
}
else
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseWebSockets();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.UseRateLimiter();

app.UseSession();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Control.Client._Imports).Assembly);

app.MapGet(ConnectionConstants.ServerHostWebsocketEndpoint, async (HttpContext ctx) =>
{
    GameConnectionService connServ = ctx.RequestServices.GetRequiredService<GameConnectionService>();
    if (!await connServ.VerifyHostConnectRequest()) return;

    WebSocket socket = await ctx.WebSockets.AcceptWebSocketAsync();
    if (socket.State != WebSocketState.Open)
        throw new InvalidOperationException("Could not establish connection");
    await connServ.HostConnected(socket);
}).RequireAuthorization();

app.Run();
