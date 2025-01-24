import { UUID } from 'node:crypto';

export type GetTrackedWorkoutResponse = {
  id: UUID;
  workoutId: UUID;
  performedAt: string;
  duration: string;
};
