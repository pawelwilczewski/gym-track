<script setup lang="ts">
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { apiClient } from '@/scripts/http/Clients';
import {
  GetWorkoutExerciseResponse,
  WorkoutExerciseKey,
} from '@/scripts/schema/Types';
import { Ref, ref } from 'vue';

const props = defineProps<{
  exerciseKey: WorkoutExerciseKey;
}>();

const exercise: Ref<GetWorkoutExerciseResponse | undefined> = ref(undefined);

async function update(): Promise<void> {
  const response = await apiClient.get(
    `api/v1/workouts/${props.exerciseKey.workoutId}/exercises/${props.exerciseKey.index}`
  );
  if (
    ErrorHandler.forResponse(response).handleFully(toastErrorHandler).wasError()
  ) {
    return;
  }
  exercise.value = response.data;
}

await update();
</script>

<template>
  <div
    v-if="exercise"
    class="mx-auto border border-border rounded-xl w-80 flex flex-col gap-6 p-8"
  >
    <h4>{{ exercise.index }}</h4>
  </div>
</template>
