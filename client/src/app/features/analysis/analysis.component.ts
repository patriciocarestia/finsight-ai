import { Component, inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, DatePipe } from '@angular/common';
import { MarkdownComponent } from 'ngx-markdown';
import { analyzePortfolio } from '../../store/analysis/analysis.actions';
import {
  selectAnalysis,
  selectAnalysisLoading,
  selectAnalysisError,
  selectAnalysisGeneratedAt,
} from '../../store/analysis/analysis.selectors';

@Component({
  selector: 'app-analysis',
  imports: [AsyncPipe, DatePipe, MarkdownComponent],
  templateUrl: './analysis.component.html',
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
