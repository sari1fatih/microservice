namespace SaleService.Application.Features.Sales.Queries.GetList;

public class GetListSaleDto
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerSurname { get; set; }

    public string? CustomerPhone { get; set; }

    public string? CustomerEmail { get; set; }

    public string? SaleName { get; set; }
    public string? SaleStatusParameterValue { get; set; }
}