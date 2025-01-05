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

export type EditWorkoutExerciseSetRequest = {
  metric: ExerciseMetric;
  reps: number;
};

export type EditWorkoutRequest = {
  name: string;
};

export type ExerciseInfoStepKey = {
  exerciseInfoId: UUID;
  index: ExerciseInfoStepIndex;
};

export type ExerciseInfoStepKeyHash = `${UUID}_${ExerciseInfoStepIndex}`;

export function hashExerciseInfoStepKey(
  key: ExerciseInfoStepKey
): ExerciseInfoStepKeyHash {
  return `${key.exerciseInfoId}_${key.index}`;
}

export type GetExerciseInfoResponse = {
  id: UUID;
  name: string;
  description: string;
  allowedMetricTypes: ExerciseMetricType;
  thumbnailUrl?: string | null;
  steps: ExerciseInfoStepKey[];
};

export type GetExerciseInfoStepResponse = {
  index: ExerciseInfoStepIndex;
  description: string;
  imageUrl?: string | null;
  displayOrder: number;
};

export type ExerciseInfoStepIndex = number;

export type GetWorkoutExerciseResponse = {
  index: WorkoutExerciseIndex;
  exerciseInfoId: UUID;
  sets: WorkoutExerciseSetKey[];
};

export type GetWorkoutExerciseSetResponse = {
  index: WorkoutExerciseSetIndex;
  metric: ExerciseMetric;
  reps: number;
  displayOrder: number;
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
  index: WorkoutExerciseIndex;
};

export type WorkoutExerciseIndex = number;

export type WorkoutExerciseKeyHash = `${UUID}_${WorkoutExerciseIndex}`;

export function hashWorkoutExerciseKey(
  key: WorkoutExerciseKey
): WorkoutExerciseKeyHash {
  return `${key.workoutId}_${key.index}`;
}

export type WorkoutExerciseSetKey = {
  workoutId: UUID;
  exerciseIndex: WorkoutExerciseIndex;
  index: WorkoutExerciseSetIndex;
};

// TODO Pawel: improve strongly typed indices:
// https://stackoverflow.com/questions/71486513/how-to-accomplish-stongly-typed-ids-in-typescript
// - not sure how it plays out with fetched data though
export type WorkoutExerciseSetIndex = number;

export type WorkoutExerciseSetKeyHash =
  `${UUID}_${WorkoutExerciseIndex}_${WorkoutExerciseSetIndex}`;

export function hashWorkoutExerciseSetKey(
  key: WorkoutExerciseSetKey
): WorkoutExerciseSetKeyHash {
  return `${key.workoutId}_${key.exerciseIndex}_${key.index}`;
}

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

// TODO Pawel: strongly typed ids
