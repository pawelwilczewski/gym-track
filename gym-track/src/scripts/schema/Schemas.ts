import { toTypedSchema } from '@vee-validate/zod';
import { z } from 'zod';
import { ExerciseMetricType } from './Types';
import { zodEnumFlagsAsStringArray } from './ZodUtils';

export const createWorkoutSchema = toTypedSchema(
  z.object({
    name: z.string().trim().min(1),
  })
);

export const createWorkoutExerciseSchema = toTypedSchema(
  z.object({
    exerciseInfoId: z.string().uuid(),
  })
);

export const createExerciseInfoSchema = toTypedSchema(
  z.object({
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
  })
);

export const createExerciseInfoStepSchema = toTypedSchema(
  z.object({
    index: z.number().int().min(0),
    description: z.string().trim().min(1),
    image: z
      .instanceof(File, { message: 'Thumbnail is required.' })
      .refine(file => file.size > 0)
      .refine(
        file => ['image/jpeg', 'image/png', 'image/gif'].includes(file.type),
        'Thumbnail must be a valid image file.'
      )
      .optional(),
  })
);

export const forgotPasswordSchema = toTypedSchema(
  z.object({
    email: z.string().email(),
  })
);

export const logInRequestSchema = toTypedSchema(
  z.object({
    email: z.string().email(),
    password: z.string(),
    rememberMe: z.boolean().default(true).optional(),
  })
);

export const signUpSchema = toTypedSchema(
  z
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
    })
);
