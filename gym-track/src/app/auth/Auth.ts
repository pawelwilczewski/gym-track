import router from '@/Router';
import { apiClient } from '@/app/http/Clients';
import { UserInfo } from '@/app/schema/Types';

export async function getCurrentUser(): Promise<UserInfo | undefined> {
  const response = await apiClient.get<UserInfo>('/auth/manage/info');
  if (response.status !== 200) {
    return undefined;
  }
  return response.data;
}

export async function logOut(): Promise<void> {
  await apiClient.post('/auth/logout', {});
  router.push('/');
}
