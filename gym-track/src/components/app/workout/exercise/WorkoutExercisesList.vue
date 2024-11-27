<script setup lang="ts">
import {
  GetWorkoutExerciseResponse,
  WorkoutExerciseKey,
} from '@/scripts/schema/Types';
import WorkoutExercise from '@/components/app/workout/exercise/WorkoutExercise.vue';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { apiClient } from '@/scripts/http/Clients';
import { ref } from 'vue';

const props = defineProps<{
  getExerciseKeys: () => WorkoutExerciseKey[];
}>();

const exercises = ref<GetWorkoutExerciseResponse[]>([]);

async function update(): Promise<void> {
  exercises.value = await Promise.all(
    props.getExerciseKeys().map(async workoutExerciseKey => {
      const response = await apiClient.get(
        `api/v1/workouts/${workoutExerciseKey.workoutId}/exercises/${workoutExerciseKey.index}`
      );
      if (
        ErrorHandler.forResponse(response)
          .handleFully(toastErrorHandler)
          .wasError()
      ) {
        return undefined;
      }

      return response.data;
    })
  );
}

update();

defineExpose({
  update,
});
</script>

<template>
  <div class="flex flex-col gap-4">
    <ol>
      <li>
        <WorkoutExercise
          v-for="exercise in exercises"
          :key="exercise.index"
          :exercise="exercise"
        />
      </li>
    </ol>
  </div>
</template>
