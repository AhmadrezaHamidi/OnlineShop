namespace Identity.Application.Services;

public interface IPasswordHasher
{
    /// <summary>Produces a one-way hash of the plain-text password.</summary>
    string Hash(string password);

    /// <summary>Returns true when the plain-text password matches the stored hash.</summary>
    bool Verify(string password, string hash);
}
