import { z } from 'zod';

export const createTrackedWorkoutSchema = z.object({
  workoutId: z.string().uuid(),
  performedAt: z.date(),
  duration: z.string().time(),
});
