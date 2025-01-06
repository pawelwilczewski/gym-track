import { z } from 'zod';
import { createExerciseInfoSchema } from './CreateExerciseInfoSchema';

export const editExerciseInfoSchema = createExerciseInfoSchema.merge(
  z.object({
    replaceThumbnailImage: z.boolean().default(false),
  })
);
