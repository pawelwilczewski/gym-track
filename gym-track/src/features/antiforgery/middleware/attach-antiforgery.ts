import { useAntiforgery } from '@/features/antiforgery/stores/use-antiforgery';
import { InternalAxiosRequestConfig } from 'axios';

export function attachAntiforgery(
  config: InternalAxiosRequestConfig<unknown>
): InternalAxiosRequestConfig<unknown> {
  const antiforgery = useAntiforgery();
  config.headers['X-CSRF-TOKEN'] = antiforgery.token;
  return config;
}
