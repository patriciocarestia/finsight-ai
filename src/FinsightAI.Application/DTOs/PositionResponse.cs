using FinsightAI.Domain.Entities;

namespace FinsightAI.Application.DTOs;

public record PositionResponse
{
    public int Id { get; init; }
    public string AssetType { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public decimal PurchasePrice { get; init; }
    public DateTime PurchaseDate { get; init; }
    public string? Notes { get; init; }
    public decimal? InterestRate { get; init; }
    public DateTime? MaturityDate { get; init; }
    public DateTime CreatedAt { get; init; }

    public static PositionResponse FromEntity(Position entity) => new()
    {
        Id = entity.Id,
        AssetType = entity.AssetType,
        Amount = entity.Amount,
        PurchasePrice = entity.PurchasePrice,
        PurchaseDate = entity.PurchaseDate,
        Notes = entity.Notes,
        InterestRate = entity.InterestRate,
        MaturityDate = entity.MaturityDate,
        CreatedAt = entity.CreatedAt
    };
}
