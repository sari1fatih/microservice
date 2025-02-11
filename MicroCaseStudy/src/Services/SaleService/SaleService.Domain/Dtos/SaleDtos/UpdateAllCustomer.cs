namespace SaleService.Domain.Dtos.SaleDtos;

public class UpdateAllCustomer
{
    public int? CustomerId { get; set; }
    public int UserId { get; set; }
    public string? CustomerName { get; set; }

    public string? CustomerSurname { get; set; }

    public string? CustomerPhone { get; set; }

    public string? CustomerEmail { get; set; }
}