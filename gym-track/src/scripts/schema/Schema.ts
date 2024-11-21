import { toTypedSchema } from '@vee-validate/zod';
import { UUID } from 'crypto';
import { z } from 'zod';

export type GetWorkoutResponse = {
  name: string;
  exercises: WorkoutExerciseKey[];
};

export type WorkoutExerciseKey = {
  workoutId: UUID;
  index: number;
};

export const createWorkoutSchema = toTypedSchema(
  z.object({
    name: z.string().trim().min(1),
  })
);

export const createExerciseSchema = toTypedSchema(
  z.object({
    name: z.string().trim().min(1),
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
