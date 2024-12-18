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
  id: UUID;
  name: string;
  description: string;
  allowedMetricTypes: ExerciseMetricType;
  thumbnailUrl?: string | null;
  steps: ExerciseInfoStepKey[];
};

export type GetExerciseInfoStepResponse = {
  index: number;
  description: string;
  imageUrl?: string | null;
};

export type GetWorkoutExerciseResponse = {
  index: number;
  exerciseInfoId: UUID;
  sets: WorkoutExerciseSetKey[];
};

export type GetWorkoutExerciseSetResponse = {
  index: number;
  metric: ExerciseMetric;
  reps: number;
};

export type GetWorkoutResponse = {
  id: UUID;
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
  $type: ExerciseMetricType.Weight;
  value: Amount;
  units: WeightUnit;
};

export enum WeightUnit {
  Kilogram = 0,
  Pound = 1,
}

export type Duration = {
  $type: ExerciseMetricType.Duration;
  time: string;
};

export type Distance = {
  $type: ExerciseMetricType.Distance;
  value: Amount;
  units: DistanceUnit;
};

export enum DistanceUnit {
  Metre = 0,
  Yard = 1,
}

export type Amount = number;
