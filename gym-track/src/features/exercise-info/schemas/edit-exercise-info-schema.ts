import { z } from 'zod';
import { createExerciseInfoSchema } from './create-exercise-info-schema';

export const editExerciseInfoSchema = createExerciseInfoSchema.merge(
  z.object({
    replaceThumbnailImage: z.boolean().default(false),
  })
);
