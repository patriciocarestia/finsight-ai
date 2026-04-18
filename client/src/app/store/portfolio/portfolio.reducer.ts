import { createFeature, createReducer, on } from '@ngrx/store';
import {
  addPosition,
  addPositionFailure,
  addPositionSuccess,
  deletePosition,
  deletePositionFailure,
  deletePositionSuccess,
  loadPositions,
  loadPositionsFailure,
  loadPositionsSuccess,
} from './portfolio.actions';
import { Position } from './portfolio.model';

export interface PortfolioState {
  positions: Position[];
  loading: boolean;
  saving: boolean;
  error: string | null;
}

const initialState: PortfolioState = {
  positions: [],
  loading: false,
  saving: false,
  error: null,
};

export const portfolioFeature = createFeature({
  name: 'portfolio',
  reducer: createReducer(
    initialState,

    on(loadPositions, (state) => ({
      ...state,
      loading: true,
      error: null,
    })),

    on(loadPositionsSuccess, (state, { positions }) => ({
      ...state,
      positions,
      loading: false,
      error: null,
    })),

    on(loadPositionsFailure, (state, { error }) => ({
      ...state,
      loading: false,
      error,
    })),

    on(addPosition, (state) => ({
      ...state,
      saving: true,
      error: null,
    })),

    on(addPositionSuccess, (state, { position }) => ({
      ...state,
      positions: [...state.positions, position],
      saving: false,
      error: null,
    })),

    on(addPositionFailure, (state, { error }) => ({
      ...state,
      saving: false,
      error,
    })),

    on(deletePosition, (state) => ({
      ...state,
      saving: true,
      error: null,
    })),

    on(deletePositionSuccess, (state, { id }) => ({
      ...state,
      positions: state.positions.filter((p) => p.id !== id),
      saving: false,
      error: null,
    })),

    on(deletePositionFailure, (state, { error }) => ({
      ...state,
      saving: false,
      error,
    })),
  ),
});
