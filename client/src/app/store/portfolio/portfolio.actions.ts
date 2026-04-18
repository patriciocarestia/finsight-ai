import { createAction, props } from '@ngrx/store';
import { AddPositionRequest, Position } from './portfolio.model';

export const loadPositions = createAction('[Portfolio] Load Positions');

export const loadPositionsSuccess = createAction(
  '[Portfolio] Load Positions Success',
  props<{ positions: Position[] }>(),
);

export const loadPositionsFailure = createAction(
  '[Portfolio] Load Positions Failure',
  props<{ error: string }>(),
);

export const addPosition = createAction(
  '[Portfolio] Add Position',
  props<{ position: AddPositionRequest }>(),
);

export const addPositionSuccess = createAction(
  '[Portfolio] Add Position Success',
  props<{ position: Position }>(),
);

export const addPositionFailure = createAction(
  '[Portfolio] Add Position Failure',
  props<{ error: string }>(),
);

export const deletePosition = createAction('[Portfolio] Delete Position', props<{ id: number }>());

export const deletePositionSuccess = createAction(
  '[Portfolio] Delete Position Success',
  props<{ id: number }>(),
);

export const deletePositionFailure = createAction(
  '[Portfolio] Delete Position Failure',
  props<{ error: string }>(),
);
