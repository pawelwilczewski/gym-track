import { z } from 'zod';
import { DistanceUnit, ExerciseMetricType, WeightUnit } from './Types';
import { zodEnumFlagsAsStringArray } from './ZodUtils';

export const createWorkoutSchema = z.object({
  name: z.string().trim().min(1),
});

export const createWorkoutExerciseSchema = z.object({
  exerciseInfoId: z.string().uuid(),
});

export const createWorkoutExerciseSetSchema = z
  .object({
    metricType: z.preprocess(
      val => Number(val),
      z.nativeEnum(ExerciseMetricType)
    ),
    reps: z.number().int().positive(),
    distanceValue: z.number().positive().optional(),
    distanceUnits: z
      .preprocess(val => Number(val), z.nativeEnum(DistanceUnit))
      .optional(),
    weightValue: z.number().positive().optional(),
    weightUnits: z
      .preprocess(val => Number(val), z.nativeEnum(WeightUnit))
      .optional(),
    time: z.string().time().optional(),
  })
  .refine(schema => {
    switch (schema.metricType) {
      case ExerciseMetricType.Distance: {
        return schema.distanceValue != null && schema.distanceUnits != null;
      }
      case ExerciseMetricType.Duration: {
        return schema.time;
      }
      case ExerciseMetricType.Weight: {
        return schema.weightValue != null && schema.weightUnits != null;
      }
      case ExerciseMetricType.All: {
        return false;
      }
    }
  }, 'All values are required.');

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

export const editExerciseInfoSchema = createExerciseInfoSchema.merge(
  z.object({
    replaceThumbnailImage: z.boolean().default(false),
  })
);

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

export const forgotPasswordSchema = z.object({
  email: z.string().email(),
});

export const logInRequestSchema = z.object({
  email: z.string().email(),
  password: z.string(),
  rememberMe: z.boolean().default(true).optional(),
});

export const signUpSchema = z
  .object({
    email: z.string().email(),
    password: z
      .string()
      .min(6, 'Password must contain at least 6 characters.')
      .refine(
        value => value.toUpperCase() !== value,
        "Password must contain at least 1 lowercase character ('a'-'z')."
      )
      .refine(
        value => value.toLowerCase() !== value,
        "Password must contain at least 1 uppercase character ('a'-'z')."
      )
      .refine(
        value => /.*\d.*/.test(value),
        "Password must contain at least 1 digit ('0'-'9')."
      )
      .refine(
        value => !/.*\s.*/.test(value),
        'Password must not contain any whitespace.'
      ),
    confirmPassword: z.string(),
  })
  .superRefine(({ confirmPassword, password }, ctx) => {
    if (confirmPassword !== password) {
      ctx.addIssue({
        code: 'custom',
        message: 'Passwords do not match.',
        path: ['confirmPassword'],
      });
    }
  });
