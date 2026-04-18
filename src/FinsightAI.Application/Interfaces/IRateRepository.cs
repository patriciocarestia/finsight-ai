using FinsightAI.Domain.Entities;

namespace FinsightAI.Application.Interfaces;

public interface IRateRepository
{
    Task<IEnumerable<ExchangeRate>> GetLatestRatesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<ExchangeRate>> GetPreviousDayRatesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<ExchangeRate>> GetRateHistoryAsync(
        string type,
        int days,
        CancellationToken cancellationToken
    );
    Task<IEnumerable<CryptoRate>> GetLatestCryptoRatesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<CryptoRate>> GetPreviousDayCryptoRatesAsync(
        CancellationToken cancellationToken
    );
    Task<IEnumerable<CryptoRate>> GetCryptoHistoryAsync(
        string symbol,
        int days,
        CancellationToken cancellationToken
    );
    Task AddExchangeRatesAsync(
        IEnumerable<ExchangeRate> rates,
        CancellationToken cancellationToken
    );
    Task AddCryptoRatesAsync(IEnumerable<CryptoRate> rates, CancellationToken cancellationToken);
}
