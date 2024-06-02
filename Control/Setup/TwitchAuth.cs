using AspNet.Security.OAuth.Twitch;
using Control.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Control.Setup;

public static class TwitchAuth
{
    public static void AddAuth(this WebApplicationBuilder builder)
    {
        builder.ConfigureAuthentication();
        builder.ConfigureAuthorization();
    }

    public static void ConfigureAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(op =>
            {
                op.Cookie.MaxAge = TimeSpan.FromDays(30);
                op.ExpireTimeSpan = TimeSpan.FromHours(2); // twitch recommends re-checking access tokens every hour
                op.AccessDeniedPath = "/Error";
            })
            .AddTwitch(op =>
            {
                op.Scope.Remove("user:read:email"); // we do not need email
                op.ClientId = builder.Configuration.GetRequiredSection("Twitch")["ClientId"]!;
                op.ClientSecret = builder.Configuration.GetRequiredSection("Twitch")["ClientSecret"]!;
                op.SaveTokens = true;
                // remove useless cookie padding
                op.ClaimActions.Remove("urn:twitch:offlineimageurl");
                op.ClaimActions.Remove("urn:twitch:description");

                op.Events.OnCreatingTicket += (ctx) =>
                {
                    bool isDev = ctx.HttpContext.RequestServices.GetService<IsDeveloperService>()
                        ?.IsDeveloper(ctx.Identity) ?? false;
                    if (isDev)
                        ctx.Identity!.AddClaim(new(ctx.Identity.RoleClaimType, "Developer"));

                    return Task.CompletedTask;
                };

                op.Validate();
            });
    }

    public static void ConfigureAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("Developer", (policy) =>
            {
                policy.AddAuthenticationSchemes(TwitchAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Developer");
            });
    }
}
