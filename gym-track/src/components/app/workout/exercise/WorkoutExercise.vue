<script setup lang="ts">
import { apiClient } from '@/scripts/http/Clients';
import Entity from '../../Entity.vue';
import {
  GetWorkoutExerciseResponse,
  WorkoutExerciseKey,
} from '@/scripts/schema/Types';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { ref } from 'vue';

const props = defineProps<{
  exerciseKey: WorkoutExerciseKey;
}>();

async function update(): Promise<void> {
  const response = await apiClient.get(
    `/api/v1/workouts/${props.exerciseKey.workoutId}/exercises/${props.exerciseKey.index}`
  );

  if (
    ErrorHandler.forResponse(response).handleFully(toastErrorHandler).wasError()
  ) {
    return;
  }

  exercise.value = response.data;
}

async function handleDelete(): Promise<void> {
  if (!exercise.value) {
    return;
  }

  const response = await apiClient.delete(
    `/api/v1/workouts/${props.exerciseKey.workoutId}/exercises/${props.exerciseKey.index}`
  );
  if (
    ErrorHandler.forResponse(response).handleFully(toastErrorHandler).wasError()
  ) {
    return;
  }

  emit('deleted', props.exerciseKey);
}

const exercise = ref<GetWorkoutExerciseResponse | undefined>(undefined);

update();

const emit = defineEmits<{
  deleted: [WorkoutExerciseKey];
}>();
</script>

<template>
  <Entity
    v-if="exercise"
    class="flex flex-col gap-6 p-8"
    @deleted="handleDelete"
  >
    <h4>{{ exercise.index }}</h4>
  </Entity>
</template>
