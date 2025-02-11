using System;
using System.Collections.Generic;

namespace SaleService.Domain.Entities;

public partial class Log
{
    public string? Message { get; set; }

    public string? MessageTemplate { get; set; }

    public string? Level { get; set; }

    public DateTime? TimeStamp { get; set; }

    public string? Exception { get; set; }

    public string? Properties { get; set; }

    public string? HttpMethod { get; set; }

    public string? Path { get; set; }

    public string? QueryParams { get; set; }

    public string? Body { get; set; }

    public int? UserId { get; set; }
}
