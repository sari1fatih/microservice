namespace SaleService.Application.Features.Parameters.Queries.GetById;

public class GetByIdParameterDto
{
    public int Id { get; set; }

    public int? ParameterGroupId { get; set; }
    
    public string ParameterGroupValue { get; set; } = null!;

    public string ParameterValue { get; set; } = null!;

    public string? ParameterValueDescription { get; set; }
}