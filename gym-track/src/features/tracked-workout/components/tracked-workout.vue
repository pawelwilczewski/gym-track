<script setup lang="ts">
import Entity from '@/features/shared/components/entity.vue';
import {
  formatDateTime,
  formatDuration,
} from '@/features/shared/utils/formatters';
import EditTrackedWorkoutForm from '@/features/tracked-workout/components/edit-tracked-workout-form.vue';
import { useTrackedWorkout } from '@/features/tracked-workout/composables/use-tracked-workout';
import { useWorkouts } from '@/features/workout/stores/use-workouts';
import { GetWorkoutResponse } from '@/features/workout/types/workout-types';
import { Clock } from 'lucide-vue-next';
import { UUID } from 'node:crypto';
import { computed, ref, watch } from 'vue';

const { trackedWorkoutId } = defineProps<{
  trackedWorkoutId: UUID;
}>();

const { trackedWorkout, destroy } = useTrackedWorkout(trackedWorkoutId);

const workouts = useWorkouts();
const workout = ref<GetWorkoutResponse | undefined>(undefined);

watch(
  trackedWorkout,
  () => {
    if (!trackedWorkout.value) {
      return;
    }
    workouts.fetchById(trackedWorkout.value.workoutId);
  },
  { immediate: true }
);

watch(
  [trackedWorkout, workouts.all],
  () => {
    if (!trackedWorkout.value) {
      return;
    }
    workout.value = workouts.all[trackedWorkout.value.workoutId];
  },
  { immediate: true }
);

const performedAt = computed(() => {
  if (!trackedWorkout.value) {
    return;
  }
  const date = new Date(trackedWorkout.value.performedAt);
  return formatDateTime(date);
});

const duration = computed(() => {
  if (!trackedWorkout.value) {
    return;
  }

  return formatDuration(trackedWorkout.value.duration);
});
</script>

<template>
  <Entity
    v-if="trackedWorkout"
    class="card"
    :editable="true"
    @deleted="destroy()"
  >
    <span class="font-bold">{{ workout?.name }}</span>
    <span>{{ performedAt }}</span>
    <span class="flex gap-1">
      <Clock class="w-4 h-4 my-auto" />
      {{ duration }}
    </span>
    <template #edit="{ closeDialog }">
      <EditTrackedWorkoutForm
        :tracked-workout-id="trackedWorkoutId"
        @edited="closeDialog()"
      />
    </template>
  </Entity>
</template>
