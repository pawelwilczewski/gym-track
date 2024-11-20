import { UUID } from 'crypto';

export type GetWorkoutResponse = {
  name: string;
  exercises: WorkoutExerciseKey[];
};

export type WorkoutExerciseKey = {
  workoutId: UUID;
  index: number;
};
