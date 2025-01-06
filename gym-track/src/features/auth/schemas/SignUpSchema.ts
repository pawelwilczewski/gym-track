import { z } from 'zod';

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
