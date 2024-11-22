import { toTypedSchema } from '@vee-validate/zod';
import { z } from 'zod';
import { ExerciseMetricType } from './Types';
import { zodEnumFlagsSchema } from './ZodUtils';

export const createWorkoutSchema = toTypedSchema(
  z.object({
    name: z.string().trim().min(1),
  })
);

export const createExerciseInfoSchema = toTypedSchema(
  z.object({
    name: z.string().trim().min(1),
    description: z.string().trim().min(1),
    allowedMetricTypes: zodEnumFlagsSchema(ExerciseMetricType),
    thumbnailImage: z
      .instanceof(File, { message: 'Thumbnail is required.' })
      .refine(file => file.size > 0)
      .refine(
        file => ['image/jpeg', 'image/png', 'image/gif'].includes(file.type),
        'Thumbnail must be a valid image file.'
      ),
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
    password: z.string().min(2, 'Password must contain at least 2 characters'),
    rememberMe: z.boolean().default(true).optional(),
  })
);

export const signUpSchema = toTypedSchema(
  z
    .object({
      email: z.string().email(),
      password: z
        .string()
        .min(2, 'Password must contain at least 2 characters'),
      confirmPassword: z
        .string()
        .min(2, 'Password must contain at least 2 characters'),
    })
    .superRefine(({ confirmPassword, password }, ctx) => {
      if (confirmPassword !== password) {
        ctx.addIssue({
          code: 'custom',
          message: 'The passwords do not match',
          path: ['confirmPassword'],
        });
      }
    })
);
