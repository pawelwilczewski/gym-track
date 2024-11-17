import router from '@/Router';
import { apiClient } from '../http/Clients';

export interface IUserInfo {
  email: string;
  isEmailConfirmed: boolean;
}

export async function getCurrentUser(): Promise<IUserInfo | undefined> {
  const response = await apiClient.get<IUserInfo>('/auth/manage/info');
  if (response.status !== 200) {
    return undefined;
  }
  return response.data;
}

export async function isLoggedIn(): Promise<boolean> {
  return (await apiClient.get('/auth/manage/info')).status === 200;
}

export async function logOut(): Promise<void> {
  await apiClient.post('/auth/logout', {});
  router.push('/');
}
