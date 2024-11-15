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
  error: string;
}

interface IValidationError {
  type: 'validation';
  errors: IInvalidField[];
}

declare global {
  interface Response {
    toResult(): ISuccess | IEmptyError | IErrorWithMessage | IValidationError;
  }
}

Object.defineProperty(Response.prototype, 'toResult', {
  value: function toResult():
    | ISuccess
    | IEmptyError
    | IErrorWithMessage
    | IValidationError {
    if (this.ok) {
      return { type: 'success' };
    }

    switch (this.status) {
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
  },
});

const response = await fetch('abc.com/api');
match(response.toResult())
  .with({ type: 'success' }, () => {})
  .with({ type: 'empty' }, () => console.log('Unknown error encountered.'))
  .with({ type: 'message', message: P.select() }, (message) =>
    console.log(message)
  )
  .with({ type: 'validation', errors: P.select() }, (errors) => {
    errors.forEach((error) => {
      console.log(error);
    });
  })
  .exhaustive();
