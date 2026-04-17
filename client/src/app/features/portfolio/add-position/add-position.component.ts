import { Component, inject, output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { AsyncPipe } from '@angular/common';
import { addPosition } from '../../../store/portfolio/portfolio.actions';
import { selectPortfolioSaving } from '../../../store/portfolio/portfolio.selectors';

const ASSET_TYPES = [
  { value: 'ARS', label: 'Pesos (ARS)' },
  { value: 'USD_OFICIAL', label: 'Dólar Oficial' },
  { value: 'USD_BLUE', label: 'Dólar Blue' },
  { value: 'USDT', label: 'USDT' },
  { value: 'BTC', label: 'Bitcoin (BTC)' },
  { value: 'ETH', label: 'Ethereum (ETH)' },
  { value: 'PLAZO_FIJO', label: 'Plazo Fijo' }
];

@Component({
  selector: 'app-add-position',
  imports: [ReactiveFormsModule, AsyncPipe],
  templateUrl: './add-position.component.html'
})
export class AddPositionComponent {
  readonly cancelled = output<void>();

  private readonly store = inject(Store);
  private readonly fb = inject(FormBuilder);

  readonly saving$ = this.store.select(selectPortfolioSaving);
  readonly assetTypes = ASSET_TYPES;

  readonly form = this.fb.nonNullable.group({
    assetType: ['USD_BLUE', Validators.required],
    amount: [0, [Validators.required, Validators.min(0.00000001)]],
    purchasePrice: [0, [Validators.required, Validators.min(0)]],
    purchaseDate: [new Date().toISOString().split('T')[0], Validators.required],
    notes: [''],
    interestRate: [null as number | null],
    maturityDate: [null as string | null]
  });

  onSubmit() {
    if (this.form.valid) {
      const v = this.form.getRawValue();
      this.store.dispatch(addPosition({
        position: {
          assetType: v.assetType,
          amount: v.amount,
          purchasePrice: v.purchasePrice,
          purchaseDate: v.purchaseDate,
          notes: v.notes || undefined,
          interestRate: v.interestRate ?? undefined,
          maturityDate: v.maturityDate ?? undefined
        }
      }));
    }
  }

  onCancel() {
    this.cancelled.emit();
  }
}
