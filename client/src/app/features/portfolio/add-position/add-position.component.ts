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
  template: `
    <div class="card">
      <h2 class="text-lg font-semibold text-white mb-4">Agregar Posición</h2>
      <form [formGroup]="form" (ngSubmit)="onSubmit()" class="space-y-4">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label class="label">Tipo de activo</label>
            <select formControlName="assetType" class="input">
              @for (type of assetTypes; track type.value) {
                <option [value]="type.value">{{ type.label }}</option>
              }
            </select>
          </div>
          <div>
            <label class="label">Cantidad</label>
            <input type="number" formControlName="amount" class="input" placeholder="0.00" step="any" />
          </div>
          <div>
            <label class="label">Precio de compra (ARS)</label>
            <input type="number" formControlName="purchasePrice" class="input" placeholder="0.00" />
          </div>
          <div>
            <label class="label">Fecha de compra</label>
            <input type="date" formControlName="purchaseDate" class="input" />
          </div>
          @if (form.get('assetType')?.value === 'PLAZO_FIJO') {
            <div>
              <label class="label">Tasa anual (%)</label>
              <input type="number" formControlName="interestRate" class="input" placeholder="140.00" />
            </div>
            <div>
              <label class="label">Fecha de vencimiento</label>
              <input type="date" formControlName="maturityDate" class="input" />
            </div>
          }
          <div class="md:col-span-2">
            <label class="label">Notas (opcional)</label>
            <input type="text" formControlName="notes" class="input" placeholder="Ej: compra en cuotas" />
          </div>
        </div>
        <div class="flex gap-3">
          <button type="submit"
                  class="btn-primary"
                  [disabled]="form.invalid || (saving$ | async)">
            @if (saving$ | async) { Guardando... } @else { Agregar }
          </button>
          <button type="button" class="btn-secondary" (click)="onCancel()">Cancelar</button>
        </div>
      </form>
    </div>
  `
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
