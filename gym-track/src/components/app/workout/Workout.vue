<script setup lang="ts">
import Entity from '../Entity.vue';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { apiClient } from '@/scripts/http/Clients';
import { GetWorkoutResponse } from '@/scripts/schema/Types';
import { ref } from 'vue';
import CreateWorkoutExercise from '@/components/app/workout/exercise/CreateWorkoutExercise.vue';
import WorkoutExercisesList from './exercise/WorkoutExercisesList.vue';
import ButtonDialog from '../misc/ButtonDialog.vue';

const props = defineProps<{
  initialWorkout: GetWorkoutResponse;
}>();

async function update(): Promise<void> {
  if (!workout.value) {
    return;
  }

  const response = await apiClient.get(`/api/v1/workouts/${workout.value.id}`);

  if (
    ErrorHandler.forResponse(response).handleFully(toastErrorHandler).wasError()
  ) {
    return;
  }

  workout.value = response.data;
  exercisesList.value?.update();
}

async function handleDelete(): Promise<void> {
  if (!workout.value) {
    return;
  }

  const response = await apiClient.delete(
    `/api/v1/workouts/${workout.value.id}`
  );
  if (
    ErrorHandler.forResponse(response).handleFully(toastErrorHandler).wasError()
  ) {
    return;
  }
  workout.value = undefined;
}

async function handleWorkoutExerciseCreated(): Promise<void> {
  createExerciseDialogOpen.value = false;
  update();
}

const workout = ref<GetWorkoutResponse | undefined>(props.initialWorkout);
const createExerciseDialogOpen = ref<boolean>(false);
const exercisesList = ref<typeof WorkoutExercisesList | undefined>(undefined);
</script>

<template>
  <Entity
    v-if="workout"
    class="mx-auto border border-border rounded-xl w-80 flex flex-col gap-6 p-8"
    @delete="handleDelete"
  >
    <h3>{{ workout.name }}</h3>
    <h4>Exercises</h4>
    <WorkoutExercisesList
      :getExerciseKeys="() => workout!.exercises ?? []"
      ref="exercisesList"
    />
    <ButtonDialog
      dialogTitle="Create Workout Exercise"
      v-model:open="createExerciseDialogOpen"
    >
      <template #button>Add Exercise</template>
      <template #dialog
        ><CreateWorkoutExercise
          :workout="workout"
          @created="handleWorkoutExerciseCreated"
      /></template>
    </ButtonDialog>
  </Entity>
</template>
