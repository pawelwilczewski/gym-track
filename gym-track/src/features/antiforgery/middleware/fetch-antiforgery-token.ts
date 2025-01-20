import { useAntiforgery } from '@/features/antiforgery/stores/use-antiforgery';

export function fetchAntiforgeryToken(): void {
  const antiforgery = useAntiforgery();
  antiforgery.fetchToken();
}
