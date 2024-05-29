using Microsoft.AspNetCore.Authentication.Cookies;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IServiceCollection services = builder.Services;
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(op =>
    {
        op.Cookie.MaxAge = TimeSpan.FromDays(30);
        op.Cookie.IsEssential = true;
    })
    .AddTwitch(op =>
    {
        op.Scope.Remove("user:read:email"); // we do not need email
        op.ClientId = builder.Configuration.GetRequiredSection("Twitch")["ClientId"]!;
        op.ClientSecret = builder.Configuration.GetRequiredSection("Twitch")["ClientSecret"]!;

        op.Validate();
    });
services.AddAuthorizationBuilder()
    .AddPolicy("Developer", (policy) =>
    {
        policy.AddAuthenticationSchemes("Twitch");
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(ctx =>
        {
            List<string> allowedUsers = builder.Configuration.GetRequiredSection("Developers").Get<List<string>>()
                ?? throw new InvalidOperationException("Developers must be a list of Twitch usernames (not display names)");
            return allowedUsers.Contains(ctx.User.Identity?.Name!);
        });
    });
services.AddRazorPages(op =>
{
    // anonymous by default
    //op.Conventions.AuthorizePage("/Privacy", "Developer"); // defined as an attribute on the page model
    //op.Conventions.AllowAnonymousToFolder("/"); // overrides all inner AuthorizePage so don't use it
    //op.Conventions.AllowAnonymousToPage("/Index");
    //op.Conventions.AllowAnonymousToPage("/Error");
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// "api"
RouteGroupBuilder apiGroup = app.MapGroup("api");
apiGroup.MapGet("/whoami", async (ctx) => await ctx.Response.WriteAsync(ctx.User.Identity?.Name ?? "Anonymous"))
    .AllowAnonymous();
apiGroup.MapGet("/dev", () => "buh")
    .RequireAuthorization("Developer");

app.Run();
