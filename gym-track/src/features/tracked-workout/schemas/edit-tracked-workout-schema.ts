import { z } from 'zod';

export const editTrackedWorkoutSchema = z.object({
  performedAt: z.date(),
  duration: z.string().time(),
});
