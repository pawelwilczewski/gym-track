import { useLocalStorage } from '@vueuse/core';
import { defineStore } from 'pinia';

export const useLockMode = defineStore('lockMode', () => {
  const isLocked = useLocalStorage('lockMode', false);

  function toggle(): void {
    isLocked.value = !isLocked.value;
  }

  return {
    isLocked,
    toggle,
  };
});
