namespace SaleService.Application.Features.SaleDetails.Queries.GetById;

public class GetByIdSaleDetailDto
{
    public int Id { get; set; }

    public int? SaleStatusParameterId { get; set; }
    public string SaleStatusParameterValue { get; set; }

    public string? Note { get; set; }
}