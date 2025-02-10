namespace CustomerService.Application.Features.CustomerNote.Queries.GetById;

public class GetByIdCustomerNoteDto
{
    public int Id { get; set; }
    public string CustomerId { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string CustomerSurname { get; set; } = null!;
    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;
    public string? Note { get; set; }

   
}