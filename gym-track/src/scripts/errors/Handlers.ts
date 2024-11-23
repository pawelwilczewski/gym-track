import { toast } from '@/components/ui/toast';
import { ResponseResult, toResult } from './ResponseResult';
import { match, P } from 'ts-pattern';
import { FormContext, Path } from 'vee-validate';
import { AxiosResponse } from 'axios';

export function toastErrorHandler(result: ResponseResult): boolean {
  let handled: boolean = false;

  match(result)
    .with({ type: 'success' }, () => {
      handled = false;
    })
    .with({ type: 'empty' }, () =>
      toast({
        title: 'Error',
        description: 'Unknown error encountered.',
        variant: 'destructive',
      })
    )
    .with({ type: 'message', message: P.select() }, message =>
      toast({
        title: 'Error',
        description: message,
        variant: 'destructive',
      })
    )
    .with({ type: 'validation', errors: P.select() }, errors => {
      errors.forEach(error => {
        toast({
          title: `Validation Error for '${error.field}'`,
          description: error.error,
          variant: 'destructive',
        });
      });
    })
    .exhaustive();

  return handled;
}

export function formErrorHandler(
  result: ResponseResult,
  form: FormContext
): boolean {
  let handled: boolean = false;

  match(result).with({ type: 'validation', errors: P.select() }, errors => {
    errors.forEach(error => {
      form.setFieldError(error.field as Path<typeof form.values>, error.error);
    });
    handled = true;
  });

  return handled;
}

export type ErrorHandler<TData = void> = TData extends void
  ? (result: ResponseResult) => boolean
  : (result: ResponseResult, data: TData) => boolean;

export class ResultErrorHandler {
  private result: ResponseResult;
  private handlers: [any, any][] = [];

  static fromResult(result: ResponseResult): ResultErrorHandler {
    return new ResultErrorHandler(result);
  }

  static fromResponse(response: AxiosResponse): ResultErrorHandler {
    return new ResultErrorHandler(toResult(response));
  }

  private constructor(result: ResponseResult) {
    this.result = result;
  }

  public then<TData = void>(
    handler: ErrorHandler<TData>,
    data?: TData
  ): ResultErrorHandler {
    this.handlers.push([handler, data]);
    return this;
  }

  public handle(): boolean {
    return this.handlers.some(handlerWithData =>
      handlerWithData[0](this.result, handlerWithData[1])
    );
  }
}
