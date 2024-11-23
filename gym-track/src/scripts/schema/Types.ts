import { UUID } from 'crypto';

export type UserInfo = {
  email: string;
  isEmailConfirmed: boolean;
};

export enum ExerciseMetricType {
  Weight = 1 << 0,
  Duration = 1 << 1,
  Distance = 1 << 2,
  All = ~(~0 << 3),
}

export type CreateWorkoutExerciseRequest = {
  index: number;
  exerciseInfoId: UUID;
};

export type CreateWorkoutExerciseSetRequest = {
  index: number;
  metric: ExerciseMetric;
  reps: number;
};

export type CreateWorkoutRequest = {
  name: string;
};

export type EditExerciseInfoRequest = {
  name: string;
  description: string;
  allowedMetricTypes: ExerciseMetricType;
};

export type EditExerciseInfoStepRequest = {
  description: string;
};

export type EditWorkoutExerciseRequest = {
  index: number;
};

export type EditWorkoutExerciseSetRequest = {
  metric: ExerciseMetric;
  reps: number;
};

export type EditWorkoutRequest = {
  name: string;
};

export type ExerciseInfoStepKey = {
  exerciseInfoId: UUID;
  index: number;
};

export type GetExerciseInfoResponse = {
  name: string;
  description: string;
  allowedMetricTypes: ExerciseMetricType;
  thumbnailUrl: string;
  steps: ExerciseInfoStepKey[];
};

export type GetExerciseInfoStepResponse = {
  index: number;
  description: string;
  imageUrl?: string | null;
};

export type GetWorkoutExerciseResponse = {
  index: number;
  sets: WorkoutExerciseSetKey[];
};

export type GetWorkoutExerciseSetResponse = {
  index: number;
  metric: ExerciseMetric;
  reps: number;
};

export type GetWorkoutResponse = {
  name: string;
  exercises: WorkoutExerciseKey[];
};

export type GetWorkoutsResponse = {
  workouts: GetWorkoutResponse[];
};

export type WorkoutExerciseKey = {
  workoutId: UUID;
  index: number;
};

export type WorkoutExerciseSetKey = {
  workoutId: UUID;
  exerciseIndex: number;
  index: number;
};

export type ExerciseMetric = Weight | Duration | Distance;

export type Weight = {
  type: 'Weight';
  value: Amount;
  units: WeightUnit;
};

export enum WeightUnit {
  Kilogram = 'Kilogram',
  Pound = 'Pound',
}

export type Duration = {
  type: 'Duration';
  time: string;
};

export type Distance = {
  type: 'Distance';
  value: Amount;
  units: DistanceUnit;
};

export enum DistanceUnit {
  Metre = 'Metre',
  Yard = 'Yard',
}

export type Amount = number;
