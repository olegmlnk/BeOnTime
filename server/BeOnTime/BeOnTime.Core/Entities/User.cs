using BeOnTime.Core.Base;
using BeOnTime.Core.Enums;

namespace BeOnTime.Core.Entities;

public class User : BaseEntity
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string  PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.User;
}