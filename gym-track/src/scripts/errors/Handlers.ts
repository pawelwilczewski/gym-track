import { toast } from '@/components/ui/toast';
import { ResponseResult } from './ResponseResult';
import { match, P } from 'ts-pattern';
import { FormContext, Path } from 'vee-validate';

export function toastErrorHandler(result: ResponseResult): boolean {
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
      errors.forEach(error => {
        toast({
          title: `Validation Error for '${error.field}'`,
          description: error.error,
          variant: 'destructive',
        });
      });
    })
    .exhaustive();

  return true;
}

export function formErrorHandler(
  result: ResponseResult,
  form: FormContext
): boolean {
  let handled: boolean = false;

  match(result)
    .with({ type: 'success' }, () => {
      handled = true;
    })
    .with({ type: 'validation', errors: P.select() }, errors => {
      errors.forEach(error => {
        form.setFieldError(
          error.field as Path<typeof form.values>,
          error.error
        );
      });
      handled = true;
    });

  return handled;
}
