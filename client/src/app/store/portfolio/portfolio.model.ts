export interface Position {
  id: number;
  assetType: string;
  amount: number;
  purchasePrice: number;
  purchaseDate: string;
  notes?: string;
  interestRate?: number;
  maturityDate?: string;
  createdAt: string;
}

export interface AddPositionRequest {
  assetType: string;
  amount: number;
  purchasePrice: number;
  purchaseDate: string;
  notes?: string;
  interestRate?: number;
  maturityDate?: string;
}
