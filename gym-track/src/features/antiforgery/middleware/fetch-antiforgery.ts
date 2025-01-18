import { useAntiforgery } from '@/features/antiforgery/stores/use-antiforgery';

export function refetchAntiforgeryToken(): void {
  const antiforgery = useAntiforgery();
  antiforgery.fetchToken();
}
