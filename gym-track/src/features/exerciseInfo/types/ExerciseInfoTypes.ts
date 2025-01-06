import { UUID } from 'crypto';

export enum ExerciseMetricType {
  Weight = 1 << 0,
  Duration = 1 << 1,
  Distance = 1 << 2,
  All = ~(~0 << 3),
}

export type EditExerciseInfoRequest = {
  name: string;
  description: string;
  allowedMetricTypes: ExerciseMetricType;
};

export type EditExerciseInfoStepRequest = {
  description: string;
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
