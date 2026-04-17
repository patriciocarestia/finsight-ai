import { Component, inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, DatePipe } from '@angular/common';
import { MarkdownComponent } from 'ngx-markdown';
import { analyzePortfolio } from '../../store/analysis/analysis.actions';
import {
  selectAnalysis,
  selectAnalysisLoading,
  selectAnalysisError,
  selectAnalysisGeneratedAt
} from '../../store/analysis/analysis.selectors';

@Component({
  selector: 'app-analysis',
  imports: [AsyncPipe, DatePipe, MarkdownComponent],
  template: `
    <div class="space-y-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-3xl font-bold text-white">Análisis IA</h1>
          <p class="text-slate-400 mt-1 text-sm">Análisis de tu portfolio con Gemini 2.5 Flash</p>
        </div>
        <button class="btn-primary flex items-center gap-2"
                (click)="onAnalyze()"
                [disabled]="loading$ | async">
          @if (loading$ | async) {
            <span class="animate-spin">&#9696;</span> Analizando...
          } @else {
            &#10024; Analizar portfolio
          }
        </button>
      </div>

      @if (error$ | async; as error) {
        <div class="bg-red-900/30 border border-red-700 text-red-400 rounded-lg px-4 py-3 text-sm">{{ error }}</div>
      }

      @if (loading$ | async) {
        <div class="card">
          <div class="flex items-center gap-3 mb-4">
            <div class="animate-spin text-blue-400 text-xl">&#9696;</div>
            <span class="text-slate-300">Gemini está analizando tu portfolio...</span>
          </div>
          <div class="space-y-2">
            @for (i of [1,2,3,4,5]; track i) {
              <div class="h-4 bg-slate-700 rounded animate-pulse" [style.width]="(60 + i * 8) + '%'"></div>
            }
          </div>
        </div>
      }

      @if (analysis$ | async; as analysis) {
        <div class="card">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-lg font-semibold text-white">Resultado del análisis</h2>
            @if (generatedAt$ | async; as generatedAt) {
              <span class="text-slate-500 text-xs">{{ generatedAt | date:'dd/MM/yyyy HH:mm' }}</span>
            }
          </div>
          <markdown class="markdown-analysis" [data]="analysis" />
        </div>
      }

      @if (!(analysis$ | async) && !(loading$ | async)) {
        <div class="card text-center py-16">
          <div class="text-5xl mb-4">&#129302;</div>
          <h2 class="text-xl font-semibold text-white mb-2">Tu analista financiero IA</h2>
          <p class="text-slate-400 max-w-md mx-auto text-sm">
            Hacé clic en "Analizar portfolio" para obtener un análisis personalizado de tus inversiones,
            comparado contra la inflación y el dólar blue.
          </p>
        </div>
      }
    </div>
  `
})
export class AnalysisComponent {
  private readonly store = inject(Store);

  readonly analysis$ = this.store.select(selectAnalysis);
  readonly loading$ = this.store.select(selectAnalysisLoading);
  readonly error$ = this.store.select(selectAnalysisError);
  readonly generatedAt$ = this.store.select(selectAnalysisGeneratedAt);

  onAnalyze() {
    this.store.dispatch(analyzePortfolio());
  }
}
