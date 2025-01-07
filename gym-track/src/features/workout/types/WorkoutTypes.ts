import { ExerciseMetric } from '@/features/exerciseInfo/types/ExerciseInfoTypes';
import { UUID } from 'crypto';

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

export type EditWorkoutExerciseSetRequest = {
  metric: ExerciseMetric;
  reps: number;
};

export type EditWorkoutRequest = {
  name: string;
};

export type GetWorkoutExerciseResponse = {
  index: WorkoutExerciseIndex;
  exerciseInfoId: UUID;
  sets: WorkoutExerciseSetKey[];
  displayOrder: number;
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
