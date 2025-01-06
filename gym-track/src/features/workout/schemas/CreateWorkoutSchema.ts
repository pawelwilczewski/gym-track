import { z } from 'zod';

export const createWorkoutSchema = z.object({
  name: z.string().trim().min(1),
});
