import { z } from 'zod';
import { createExerciseInfoStepSchema } from './CreateExerciseInfoStepSchema';

export const editExerciseInfoStepSchema = createExerciseInfoStepSchema.merge(
  z.object({
    replaceImage: z.boolean().default(false),
  })
);
