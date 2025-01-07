import { AxiosResponse } from 'axios';
import { ResponseResult } from './response-result';
import { uncapitalize } from '@/features/shared/utils/string-utils';
import { match, P } from 'ts-pattern';

export function responseToResult(response: AxiosResponse): ResponseResult {
  if (response.status >= 200 && response.status < 300) {
    return { type: 'success' };
  }

  switch (response.status) {
    case 400: {
      if (!response.data.errors) {
        return { type: 'message', message: 'Bad request submitted.' };
      }

      const validationErrors: Map<string, string> = new Map();
      for (const errorProperty in response.data.errors) {
        const fieldName = match(errorProperty)
          // handling some common ASP.NET rule violations so they display correctly for forms
          // (i.e. 'PasswordRequiresDigit': '...', 'PasswordTooShort': '...')
          .with(P.string.startsWith('Password'), () => 'password')
          .with('DuplicateUserName', () => 'email')
          .otherwise(() => uncapitalize(errorProperty));

        const fieldErrors = validationErrors.get(fieldName) ?? '';
        const errorsToAdd = response.data.errors[errorProperty];
        validationErrors.set(
          fieldName,
          `${fieldErrors}\n${errorsToAdd.join('\n')}`.trim()
        );
      }

      return {
        type: 'validation',
        errors: Array.from(validationErrors, ([field, error]) => ({
          field,
          error,
        })),
      };
    }
    case 401: {
      return { type: 'auth' };
    }
    case 404: {
      return { type: 'message', message: 'Asset not found.' };
    }
    case 500: {
      return { type: 'message', message: 'Error processing request.' };
    }
  }

  return { type: 'empty' };
}
