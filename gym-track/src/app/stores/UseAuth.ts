import { defineStore } from 'pinia';
import { apiClient } from '../http/Clients';
import { computed, ref } from 'vue';
import router from '@/Router';
import { UserInfo } from '@/app/schema/Types';

export const useAuth = defineStore('auth', () => {
  const currentUser = ref<UserInfo | null>(null);
  const isLoggedIn = computed(() => currentUser.value !== null);

  async function fetchCurrentUser(): Promise<boolean> {
    const response = await apiClient.get<UserInfo>('/auth/manage/info');

    if (response.status !== 200) {
      return false;
    }

    currentUser.value = response.data;

    return true;
  }

  async function logOut(): Promise<boolean> {
    const response = await apiClient.post('/auth/logout', {});
    if (response.status !== 200) {
      return false;
    }

    currentUser.value = null;
    router.push('/');

    return true;
  }

  return {
    currentUser,
    isLoggedIn,
    fetchCurrentUser,
    logOut,
  };
});
