using Core.Persistance.Repository;

namespace SaleService.Domain.Entities;

public partial class SaleDetail:Entity
{
    public int Id { get; set; }

    public int SaleId { get; set; }

    public int SaleStatusParameterId { get; set; }

    public string? Note { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Guid RecordGuid { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Sale Sale { get; set; } = null!;

    public virtual Parameter SaleStatusParameter { get; set; } = null!;
}
