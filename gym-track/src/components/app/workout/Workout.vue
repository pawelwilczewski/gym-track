<script setup lang="ts">
import Entity from '../Entity.vue';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { apiClient } from '@/scripts/http/Clients';
import { GetWorkoutResponse, WorkoutExerciseKey } from '@/scripts/schema/Types';
import { ref, watch } from 'vue';
import CreateWorkoutExercise from '@/components/app/workout/exercise/CreateWorkoutExercise.vue';
import WorkoutExercisesList from './exercise/WorkoutExercisesList.vue';
import ButtonDialog from '../misc/ButtonDialog.vue';
import { UUID } from 'crypto';

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

  emit('deleted', props.initialWorkout.id);
}

const workout = ref<GetWorkoutResponse | undefined>(props.initialWorkout);

const exerciseKeys = ref<WorkoutExerciseKey[]>([]);
watch(
  workout,
  () => {
    exerciseKeys.value = workout.value?.exercises ?? [];
  },
  { immediate: true }
);

const emit = defineEmits<{ deleted: [UUID] }>();
</script>

<template>
  <Entity
    v-if="workout"
    class="mx-auto border border-border rounded-xl w-80 flex flex-col gap-6 p-8"
    @deleted="handleDelete"
  >
    <h3>{{ workout.name }}</h3>
    <h4>Exercises</h4>
    <WorkoutExercisesList
      v-if="exerciseKeys"
      :exerciseKeys="exerciseKeys"
      ref="exercisesList"
      @exercise-deleted="
        key => {
          exerciseKeys = exerciseKeys.filter(
            exerciseKey => exerciseKey !== key
          );
        }
      "
    />
    <ButtonDialog dialogTitle="Create Workout Exercise">
      <template #button>Add Exercise</template>
      <template #dialog="{ closeDialog }"
        ><CreateWorkoutExercise
          :workout="workout"
          @created="
            update();
            closeDialog();
          "
      /></template>
    </ButtonDialog>
  </Entity>
</template>
