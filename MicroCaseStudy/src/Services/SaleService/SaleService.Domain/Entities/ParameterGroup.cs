using Core.Persistance.Repository;

namespace SaleService.Domain.Entities;

public partial class ParameterGroup:Entity
{
    public int Id { get; set; }

    public string ParameterGroupValue { get; set; } = null!;

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Guid RecordGuid { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Parameter> Parameters { get; set; } = new List<Parameter>();
}
