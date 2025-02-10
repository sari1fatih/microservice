namespace CustomerService.Application.Features.Customer.Queries.GetList;

public class GetListCustomerDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Surname { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Company { get; set; } = null!;
}