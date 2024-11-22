import { computed, ref, Ref } from 'vue';
import { IUserInfo } from '../auth/Auth';

export const currentUser: Ref<IUserInfo | undefined> = ref(undefined);
export const isLoggedIn = computed(() => currentUser.value !== undefined);
