namespace FinsightAI.Domain.Entities;

public class Position
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string AssetType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string? Notes { get; set; }
    // For Plazo Fijo
    public decimal? InterestRate { get; set; }
    public DateTime? MaturityDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
