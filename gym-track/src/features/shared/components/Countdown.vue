<script setup lang="ts">
import { roundToMultiple } from '@/features/shared/utils/math-utils';
import Timer from '@/features/shared/utils/time/timer';
import { ref } from 'vue';

const { totalDurationSeconds, tickIntervalSeconds } = defineProps<{
  totalDurationSeconds: number;
  tickIntervalSeconds: number;
}>();

const emit = defineEmits<{
  complete: [];
}>();

const timeLeft = ref(totalDurationSeconds);

const timer: Timer = new Timer(
  totalDurationSeconds,
  tickIntervalSeconds,
  (timeLeftRaw: number) => {
    timeLeft.value = roundToMultiple(timeLeftRaw, tickIntervalSeconds);
  },
  () => {
    timeLeft.value = totalDurationSeconds;
    emit('complete');
  }
);

const start: () => void = () => {
  timer.start();
};

defineExpose({
  start,
});
</script>

<template>
  <span>{{ timeLeft }}</span>
</template>
