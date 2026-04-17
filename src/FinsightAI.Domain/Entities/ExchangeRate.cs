namespace FinsightAI.Domain.Entities;

public class ExchangeRate
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty; // oficial, blue, mep, ccl, cripto
    public decimal Buy { get; set; }
    public decimal Sell { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}
