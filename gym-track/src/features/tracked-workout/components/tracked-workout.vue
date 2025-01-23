<script setup lang="ts">
import { useTrackedWorkout } from '@/features/tracked-workout/composables/use-tracked-workout';
import { UUID } from 'node:crypto';

const { trackedWorkoutId } = defineProps<{
  trackedWorkoutId: UUID;
}>();

const { trackedWorkout } = useTrackedWorkout(trackedWorkoutId);
</script>

<template>
  <div v-if="trackedWorkout" class="card">
    {{ trackedWorkout.workoutId }}
    {{ trackedWorkout.performedAt }}
    {{ trackedWorkout.duration }}
  </div>
</template>
