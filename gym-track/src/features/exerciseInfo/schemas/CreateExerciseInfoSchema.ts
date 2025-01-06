import { z } from 'zod';
import { ExerciseMetricType } from '@/features/exerciseInfo/types/ExerciseInfoTypes';
import { zodEnumFlagsAsStringArray } from '@/features/shared/utils/ZodUtils';

export const createExerciseInfoSchema = z.object({
  name: z.string().trim().min(1),
  description: z.string().trim().min(1),
  allowedMetricTypes: zodEnumFlagsAsStringArray(ExerciseMetricType),
  thumbnailImage: z
    .instanceof(File, { message: 'Thumbnail is required.' })
    .refine(file => file.size > 0)
    .refine(
      file => ['image/jpeg', 'image/png', 'image/gif'].includes(file.type),
      'Thumbnail must be a valid image file.'
    )
    .optional(),
});
