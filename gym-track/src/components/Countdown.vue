<script setup lang="ts">
import { roundToMultiple } from '@/scripts/math/RoundingExtensions';
import Timer from '@/scripts/time/Timer';
import { ref } from 'vue';

const props = defineProps<{
  totalDurationSeconds: number;
  tickIntervalSeconds: number;
}>();

const emit = defineEmits<{
  complete: [];
}>();

const timeLeft = ref(props.totalDurationSeconds);

const timer: Timer = new Timer(
  props.totalDurationSeconds,
  props.tickIntervalSeconds,
  (timeLeftRaw: number) => {
    timeLeft.value = roundToMultiple(timeLeftRaw, props.tickIntervalSeconds);
  },
  () => {
    timeLeft.value = props.totalDurationSeconds;
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
