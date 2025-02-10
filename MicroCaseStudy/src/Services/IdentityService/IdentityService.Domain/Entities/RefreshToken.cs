using Core.Persistance.Repository;

namespace IdentityService.Domain.Entities;

public partial class RefreshToken:Entity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime ExpiresDate { get; set; }

    public string CreatedByIp { get; set; } = null!;

    public DateTime? RevokedDate { get; set; }

    public string? RevokedByIp { get; set; }

    public string? ReplacedByJti { get; set; }

    public string? ReasonRevoked { get; set; }

    public string Jti { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Guid RecordGuid { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? DeletedByNavigation { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
