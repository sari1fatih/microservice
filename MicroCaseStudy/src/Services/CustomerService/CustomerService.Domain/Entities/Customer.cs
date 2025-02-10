using Core.Persistance.Repository;

namespace CustomerService.Domain.Entities;

public partial class Customer : Entity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Surname { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Company { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Guid RecordGuid { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<CustomerNote> CustomerNotes { get; set; } = new List<CustomerNote>();
}
