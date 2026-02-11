// Services/UserSession.cs
namespace SecureSolution2.Services;

public sealed class UserSession
{
    public string? DisplayName { get; private set; }

    public bool IsSet => DisplayName is not null;

    public void SetName(string name) => DisplayName = name.Trim();
}
