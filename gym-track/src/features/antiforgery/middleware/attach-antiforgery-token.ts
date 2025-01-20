import { useAntiforgery } from '@/features/antiforgery/stores/use-antiforgery';
import { InternalAxiosRequestConfig } from 'axios';

export function attachAntiforgeryToken(
  config: InternalAxiosRequestConfig<unknown>
): InternalAxiosRequestConfig<unknown> {
  const antiforgery = useAntiforgery();

  if (
    antiforgery.token &&
    config.method &&
    config.method.toLowerCase() === 'post' &&
    config.data &&
    // just check if not json because Content-Type is sometimes undefined
    config.headers['Content-Type'] !== 'application/json' &&
    config.data instanceof FormData
  ) {
    // If it's FormData, append the token
    config.data.append('__RequestVerificationToken', antiforgery.token);
  }

  return config;
}
