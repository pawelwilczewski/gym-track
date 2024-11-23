import { computed, ref, Ref } from 'vue';
import { UserInfo } from '@/scripts/schema/Types';

export const currentUser: Ref<UserInfo | undefined> = ref(undefined);
export const isLoggedIn = computed(() => currentUser.value !== undefined);
