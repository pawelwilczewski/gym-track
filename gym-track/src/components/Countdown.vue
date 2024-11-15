<script setup lang="ts">
import Timer from '@/scripts/Time/Timer';
import { ref } from 'vue';
import '@/scripts/Math/RoundingExtensions';

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
    timeLeft.value = timeLeftRaw.roundToMultiple(props.tickIntervalSeconds);
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
