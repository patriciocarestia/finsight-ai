export interface ExchangeRate {
  id: number;
  type: string;
  buy: number;
  sell: number;
  changePercent: number;
  recordedAt: string;
}

export interface CryptoRate {
  id: number;
  symbol: string;
  priceUsd: number;
  priceArs: number;
  changePercent: number;
  recordedAt: string;
}

export interface LatestRatesResponse {
  exchangeRates: ExchangeRate[];
  cryptoRates: CryptoRate[];
  fetchedAt: string;
}
