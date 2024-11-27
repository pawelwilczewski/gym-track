<script setup lang="ts">
import Button from '@/components/ui/button/Button.vue';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { apiClient } from '@/scripts/http/Clients';
import { GetWorkoutResponse } from '@/scripts/schema/Types';
import { ref } from 'vue';
import Dialog from '@/components/ui/dialog/Dialog.vue';
import CreateWorkoutExercise from '@/components/app/workout/exercise/CreateWorkoutExercise.vue';
import WorkoutExercisesList from './exercise/WorkoutExercisesList.vue';
import ButtonDialog from '../misc/ButtonDialog.vue';

const props = defineProps<{
  initialWorkout: GetWorkoutResponse;
}>();

async function update(): Promise<void> {
  const response = await apiClient.get(
    `/api/v1/workouts/${props.initialWorkout.id}`
  );

  if (
    ErrorHandler.forResponse(response).handleFully(toastErrorHandler).wasError()
  ) {
    return;
  }

  workout.value = response.data;
  exercisesList.value?.update();
}

async function remove(): Promise<void> {
  if (!workout) {
    return;
  }
  const response = await apiClient.delete(
    `/api/v1/workouts/${props.initialWorkout.id}`
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
  <div
    v-if="workout"
    class="mx-auto border border-border rounded-xl w-80 flex flex-col gap-6 p-8"
  >
    <h3>{{ workout.name }}</h3>
    <Button @click="remove">Delete</Button>
    <h4>Exercises</h4>
    <WorkoutExercisesList
      :getExerciseKeys="() => workout?.exercises ?? []"
      ref="exercisesList"
    />
    <Dialog v-model:open="createExerciseDialogOpen">
      <ButtonDialog
        buttonText="Add Exercise"
        dialogTitle="Create Workout Exercise"
        v-model:open="createExerciseDialogOpen"
      >
        <CreateWorkoutExercise
          :workout="workout"
          v-on:created="handleWorkoutExerciseCreated"
        />
      </ButtonDialog>
    </Dialog>
  </div>
</template>
