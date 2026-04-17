namespace FinsightAI.Domain.Entities;

public class CryptoRate
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty; // BTC, ETH
    public decimal PriceUsd { get; set; }
    public decimal PriceArs { get; set; }
    public decimal ChangePercent24h { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}
