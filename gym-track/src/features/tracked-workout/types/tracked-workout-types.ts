import { UUID } from 'node:crypto';

export type GetTrackedWorkoutResponse = {
  id: UUID;
  workoutId: UUID;
  performedAt: Date;
  duration: string;
};
