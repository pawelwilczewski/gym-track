import { AxiosResponse } from 'axios';

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
      return {
        type: 'validation',
        errors: [
          { field: 'Field A', error: 'Too long (5 chars max).' },
          { field: 'Field B', error: 'Invalid email.' },
        ],
      };
    }
  }

  return { type: 'empty' };
}
