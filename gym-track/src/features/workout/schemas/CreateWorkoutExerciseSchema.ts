import { z } from 'zod';

export const createWorkoutExerciseSchema = z.object({
  exerciseInfoId: z.string().uuid(),
});
