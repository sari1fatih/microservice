using Core.Persistance.Repository;

namespace IdentityService.Domain.Entities;

public partial class User:Entity
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Surname { get; set; }

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public string Email { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Guid RecordGuid { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User? DeletedByNavigation { get; set; }

    public virtual ICollection<User> InverseDeletedByNavigation { get; set; } = new List<User>();

    public virtual ICollection<User> InverseUpdatedByNavigation { get; set; } = new List<User>();

    public virtual ICollection<RefreshToken> RefreshTokenCreatedByNavigations { get; set; } = new List<RefreshToken>();

    public virtual ICollection<RefreshToken> RefreshTokenDeletedByNavigations { get; set; } = new List<RefreshToken>();

    public virtual ICollection<RefreshToken> RefreshTokenUpdatedByNavigations { get; set; } = new List<RefreshToken>();

    public virtual ICollection<RefreshToken> RefreshTokenUsers { get; set; } = new List<RefreshToken>();

    public virtual ICollection<Role> RoleCreatedByNavigations { get; set; } = new List<Role>();

    public virtual ICollection<Role> RoleDeletedByNavigations { get; set; } = new List<Role>();

    public virtual ICollection<Role> RoleUpdatedByNavigations { get; set; } = new List<Role>();

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual ICollection<UserRole> UserRoleCreatedByNavigations { get; set; } = new List<UserRole>();

    public virtual ICollection<UserRole> UserRoleDeletedByNavigations { get; set; } = new List<UserRole>();

    public virtual ICollection<UserRole> UserRoleUpdatedByNavigations { get; set; } = new List<UserRole>();

    public virtual ICollection<UserRole> UserRoleUsers { get; set; } = new List<UserRole>();
}
