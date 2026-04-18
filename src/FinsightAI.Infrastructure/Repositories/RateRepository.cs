using FinsightAI.Application.Interfaces;
using FinsightAI.Domain.Entities;
using FinsightAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinsightAI.Infrastructure.Repositories;

public class RateRepository : IRateRepository
{
    private readonly AppDbContext context;

    public RateRepository(AppDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        this.context = context;
    }

    public async Task<IEnumerable<ExchangeRate>> GetLatestRatesAsync(CancellationToken cancellationToken)
    {
        var types = new[] { "oficial", "blue", "mep", "ccl", "cripto" };
        var result = new List<ExchangeRate>();

        foreach (var type in types)
        {
            var latest = await this.context.ExchangeRates
                .Where(r => r.Type == type)
                .OrderByDescending(r => r.RecordedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (latest is not null)
                result.Add(latest);
        }

        return result;
    }

    public async Task<IEnumerable<ExchangeRate>> GetPreviousDayRatesAsync(CancellationToken cancellationToken)
    {
        var types = new[] { "oficial", "blue", "mep", "ccl", "cripto" };
        var result = new List<ExchangeRate>();

        foreach (var type in types)
        {
            var latestDate = await this.context.ExchangeRates
                .Where(r => r.Type == type)
                .MaxAsync(r => (DateTime?)r.RecordedAt, cancellationToken);

            if (latestDate is null) continue;

            var cutoff = latestDate.Value.Date;

            var previous = await this.context.ExchangeRates
                .Where(r => r.Type == type && r.RecordedAt < cutoff)
                .OrderByDescending(r => r.RecordedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (previous is not null)
                result.Add(previous);
        }

        return result;
    }

    public async Task<IEnumerable<ExchangeRate>> GetRateHistoryAsync(string type, int days, CancellationToken cancellationToken)
    {
        var since = DateTime.UtcNow.AddDays(-days);
        return await this.context.ExchangeRates
            .Where(r => r.Type == type && r.RecordedAt >= since)
            .OrderBy(r => r.RecordedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CryptoRate>> GetLatestCryptoRatesAsync(CancellationToken cancellationToken)
    {
        var symbols = new[] { "BTC", "ETH" };
        var result = new List<CryptoRate>();

        foreach (var symbol in symbols)
        {
            var latest = await this.context.CryptoRates
                .Where(r => r.Symbol == symbol)
                .OrderByDescending(r => r.RecordedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (latest is not null)
                result.Add(latest);
        }

        return result;
    }

    public async Task<IEnumerable<CryptoRate>> GetPreviousDayCryptoRatesAsync(CancellationToken cancellationToken)
    {
        var symbols = new[] { "BTC", "ETH" };
        var result = new List<CryptoRate>();

        foreach (var symbol in symbols)
        {
            var latestDate = await this.context.CryptoRates
                .Where(r => r.Symbol == symbol)
                .MaxAsync(r => (DateTime?)r.RecordedAt, cancellationToken);

            if (latestDate is null) continue;

            var cutoff = latestDate.Value.Date;

            var previous = await this.context.CryptoRates
                .Where(r => r.Symbol == symbol && r.RecordedAt < cutoff)
                .OrderByDescending(r => r.RecordedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (previous is not null)
                result.Add(previous);
        }

        return result;
    }

    public async Task<IEnumerable<CryptoRate>> GetCryptoHistoryAsync(string symbol, int days, CancellationToken cancellationToken)
    {
        var since = DateTime.UtcNow.AddDays(-days);
        return await this.context.CryptoRates
            .Where(r => r.Symbol == symbol && r.RecordedAt >= since)
            .OrderBy(r => r.RecordedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddExchangeRatesAsync(IEnumerable<ExchangeRate> rates, CancellationToken cancellationToken)
    {
        this.context.ExchangeRates.AddRange(rates);
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddCryptoRatesAsync(IEnumerable<CryptoRate> rates, CancellationToken cancellationToken)
    {
        this.context.CryptoRates.AddRange(rates);
        await this.context.SaveChangesAsync(cancellationToken);
    }
}
