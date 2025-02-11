using Core.Persistance.Repository;

namespace SaleService.Domain.Entities;

public partial class Parameter:Entity
{
    public int Id { get; set; }

    public int? ParameterGroupId { get; set; }

    public string ParameterValue { get; set; } = null!;

    public string? ParameterValueDescription { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Guid RecordGuid { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ParameterGroup? ParameterGroup { get; set; }

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}
