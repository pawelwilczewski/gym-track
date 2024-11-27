<script setup lang="ts">
import {
  GetWorkoutExerciseResponse,
  GetWorkoutResponse,
} from '@/scripts/schema/Types';
import WorkoutExercise from '@/components/app/workout/exercise/WorkoutExercise.vue';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { apiClient } from '@/scripts/http/Clients';
import { ref } from 'vue';

const props = defineProps<{
  workout: GetWorkoutResponse;
}>();

const exercises = ref<GetWorkoutExerciseResponse[]>([]);

async function update(): Promise<void> {
  exercises.value = await Promise.all(
    props.workout.exercises.map(async workoutExerciseKey => {
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

await update();

defineExpose({
  update,
});
</script>

<template>
  <div class="flex flex-col gap-4">
    <ol>
      <li>
        <Suspense>
          <WorkoutExercise
            v-for="exercise in exercises"
            :key="exercise.index"
            :exercise="exercise"
          />
        </Suspense>
      </li>
    </ol>
  </div>
</template>
