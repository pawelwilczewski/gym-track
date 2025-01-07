import { z } from 'zod';

export const createExerciseInfoStepSchema = z.object({
  description: z.string().trim().min(1),
  image: z
    .instanceof(File, { message: 'Thumbnail is required.' })
    .refine(file => file.size > 0)
    .refine(
      file => ['image/jpeg', 'image/png', 'image/gif'].includes(file.type),
      'Thumbnail must be a valid image file.'
    )
    .optional(),
});
