import { defineStore } from 'pinia';
import { apiClient } from '@/features/shared/http/ApiClient';
import { computed, ref } from 'vue';
import router from '@/Router';
import {
  LogInRequest,
  SignUpRequest,
  UserInfo,
} from '@/features/auth/types/AuthTypes';
import { ErrorHandler } from '@/features/shared/errors/ErrorHandler';
import {
  formErrorHandler,
  invalidCredentialsErrorHandler,
  toastErrorHandler,
} from '@/features/shared/errors/Handlers';
import { FormContext } from 'vee-validate';

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

  async function signUp(
    request: SignUpRequest,
    form: FormContext
  ): Promise<boolean> {
    const response = await apiClient.post('/auth/register', {
      email: request.email,
      password: request.password,
    });

    return ErrorHandler.forResponse(response)
      .handlePartially(formErrorHandler, form)
      .handleFully(toastErrorHandler)
      .wasSuccess();
  }

  async function logIn(request: LogInRequest): Promise<boolean> {
    const response = await apiClient.post(
      `/auth/login?useCookies=true&useSessionCookies=${!request.rememberMe}`,
      {
        email: request.email,
        password: request.password,
      }
    );

    return ErrorHandler.forResponse(response)
      .handlePartially(invalidCredentialsErrorHandler)
      .handleFully(toastErrorHandler)
      .wasSuccess();
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
    signUp,
    logIn,
    logOut,
  };
});
