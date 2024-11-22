import { AxiosResponse } from 'axios';
import { match, P } from 'ts-pattern';

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
  errors: string[];
}

interface IValidationError {
  type: 'validation';
  errors: IInvalidField[];
}

export function toResult(
  response: AxiosResponse
): ISuccess | IEmptyError | IErrorWithMessage | IValidationError {
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

      const validationErrors: Map<string, string[]> = new Map();
      for (const errorProperty in response.data.errors) {
        const fieldName = match(errorProperty)
          // workaround for how ASP.NET returns validation errors for passwords
          // (i.e. 'PasswordRequiresDigit': '...', 'PasswordTooShort': '...')
          .with(P.string.startsWith('Password'), () => 'password')
          .otherwise(() => 'errorProperty');

        const fieldErrors = validationErrors.get(fieldName);
        const errorsToAdd = response.data.errors[errorProperty];
        if (!fieldErrors) {
          validationErrors.set(fieldName, errorsToAdd);
        } else {
          validationErrors.set(fieldName, fieldErrors.concat(errorsToAdd));
        }
      }

      return {
        type: 'validation',
        errors: Array.from(validationErrors, ([field, errors]) => ({
          field,
          errors,
        })),
      };
    }
  }

  return { type: 'empty' };
}
