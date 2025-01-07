import { z } from 'zod';
import { createExerciseInfoStepSchema } from './create-exercise-info-step-schema';

export const editExerciseInfoStepSchema = createExerciseInfoStepSchema.merge(
  z.object({
    replaceImage: z.boolean().default(false),
  })
);
