using System.ComponentModel.DataAnnotations;
using FinsightAI.Application.DTOs;
using MediatR;

namespace FinsightAI.Application.UseCases.Portfolio.Commands.UpdatePosition;

public class UpdatePositionCommand : IRequest<PositionResponse>
{
    public int Id { get; set; }
    public int UserId { get; set; }

    [Required]
    public string AssetType { get; set; } = string.Empty;

    [Required]
    [Range(0.00000001, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal PurchasePrice { get; set; }

    [Required]
    public DateTime PurchaseDate { get; set; }

    public string? Notes { get; set; }
    public decimal? InterestRate { get; set; }
    public DateTime? MaturityDate { get; set; }
}
