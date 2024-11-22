import { toast } from '@/components/ui/toast';
import { toResult } from './ResponseResult';
import { match, P } from 'ts-pattern';
import { FormContext, Path } from 'vee-validate';
import { AxiosResponse } from 'axios';

export function handleResponse(
  response: AxiosResponse,
  onSuccess: () => void,
  form: FormContext | undefined = undefined
): void {
  match(toResult(response))
    .with({ type: 'success' }, onSuccess)
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
        if (form) {
          form.setFieldError(
            error.field as Path<typeof form.values>,
            error.error
          );
        } else {
          toast({
            title: `Validation Error for '${error.field}'`,
            description: error.error,
            variant: 'destructive',
          });
        }
      });
    })
    .exhaustive();
}
