import { AxiosResponse } from 'axios';
import { match, P } from 'ts-pattern';
import { uncapitalize } from '../utils/StringUtils';

interface ISuccess {
  type: 'success';
}

interface IEmptyError {
  type: 'empty';
}

interface IErrorWithMessage {
  type: 'message';
  message: string;
}

interface IInvalidField {
  field: string;
  error: string;
}

interface IValidationError {
  type: 'validation';
  errors: IInvalidField[];
}

export type ResponseResult =
  | ISuccess
  | IEmptyError
  | IErrorWithMessage
  | IValidationError;

export function toResult(response: AxiosResponse): ResponseResult {
  if (response.status >= 200 && response.status < 300) {
    return { type: 'success' };
  }

  switch (response.status) {
    case 404: {
      return { type: 'message', message: 'Asset not found.' };
    }
    case 500: {
      return { type: 'message', message: 'Error processing request.' };
    }
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
  }

  return { type: 'empty' };
}
