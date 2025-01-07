import { toast } from '@/features/shared/components/ui/toast';
import { ResponseResult } from './response-result';
import { match, P } from 'ts-pattern';
import { FormContext, Path } from 'vee-validate';
import {
  FullyHandledErrorInfo,
  HandlerType,
  PartiallyHandledErrorInfo,
} from './error-handler';

export function toastErrorHandler(
  result: ResponseResult
): FullyHandledErrorInfo {
  match(result)
    .with({ type: 'success' }, () => {})
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
      for (const error of errors) {
        toast({
          title: `Validation Error for '${error.field}'`,
          description: error.error,
          variant: 'destructive',
        });
      }
    })
    .with({ type: 'auth' }, () => {
      toast({
        title: 'Unauthenticated',
        description: 'Log in before proceeding.',
        variant: 'destructive',
      });
    })
    .exhaustive();

  return { type: HandlerType.Full };
}

export function formErrorHandler(
  result: ResponseResult,
  form: FormContext
): PartiallyHandledErrorInfo {
  let handled: boolean = false;

  match(result)
    .with({ type: 'success' }, () => {
      handled = true;
    })
    .with({ type: 'validation', errors: P.select() }, errors => {
      for (const error of errors) {
        form.setFieldError(
          error.field as Path<typeof form.values>,
          error.error
        );
      }
      handled = true;
    });

  return {
    type: HandlerType.Partial,
    wasHandled: handled,
  };
}

export function invalidCredentialsErrorHandler(
  result: ResponseResult
): PartiallyHandledErrorInfo {
  if (result.type === 'auth') {
    toast({
      title: 'Error',
      description: 'Invalid username or password.',
      variant: 'destructive',
    });
    return {
      type: HandlerType.Partial,
      wasHandled: true,
    };
  }

  return {
    type: HandlerType.Partial,
    wasHandled: false,
  };
}
