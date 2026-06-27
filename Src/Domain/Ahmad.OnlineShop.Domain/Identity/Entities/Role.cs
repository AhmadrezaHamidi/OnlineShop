namespace Identity.Domain.Entities;

public sealed class Role
{
    public long Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private Role() { }

    public Role(long id, string name)
    {
        Id = id;
        Name = name;
    }
}
