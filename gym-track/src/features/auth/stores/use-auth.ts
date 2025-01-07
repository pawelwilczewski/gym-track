import { defineStore } from 'pinia';
import { apiClient } from '@/features/shared/http/api-client';
import { computed, ref } from 'vue';
import router from '@/router';
import {
  LogInRequest,
  SignUpRequest,
  UserInfo,
} from '@/features/auth/types/auth-types';
import { ErrorHandler } from '@/features/shared/errors/error-handler';
import {
  formErrorHandler,
  invalidCredentialsErrorHandler,
  toastErrorHandler,
} from '@/features/shared/errors/handlers';
import { FormContext } from 'vee-validate';

export const useAuth = defineStore('auth', () => {
  const currentUser = ref<UserInfo | undefined>(undefined);
  const isLoggedIn = computed(() => currentUser.value !== undefined);

  async function fetchCurrentUser(): Promise<boolean> {
    const response = await apiClient.get<UserInfo>('/auth/manage/info');

    if (response.status !== 200) {
      return false;
    }

    currentUser.value = response.data;

    return true;
  }

  // eslint-disable-next-line unicorn/consistent-function-scoping
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

  // eslint-disable-next-line unicorn/consistent-function-scoping
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

    currentUser.value = undefined;
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
