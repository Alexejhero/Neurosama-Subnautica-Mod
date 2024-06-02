using System.Security.Principal;

namespace Control.Services;

public class IsDeveloperService(IConfiguration config)
{
    public bool IsDeveloper(string? username)
    {
        if (string.IsNullOrEmpty(username))
            return false;

        List<string>? allowedUsers = config.GetRequiredSection("Developers").Get<List<string>>();
        return allowedUsers?.Contains(username) ?? false;
    }

    public bool IsDeveloper(IIdentity? identity)
        => IsDeveloper(identity?.Name);
}
