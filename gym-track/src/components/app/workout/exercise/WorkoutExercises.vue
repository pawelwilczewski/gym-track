<script setup lang="ts">
import { GetWorkoutResponse } from '@/scripts/schema/Types';
import WorkoutExercise from '@/components/app/workout/exercise/WorkoutExercise.vue';

const props = defineProps<{
  workout: GetWorkoutResponse;
}>();
</script>

<template>
  <div class="flex flex-col gap-4">
    <ol>
      <li>
        <Suspense>
          <WorkoutExercise
            v-for="exerciseKey in props.workout.exercises"
            :exerciseKey="exerciseKey"
          />
        </Suspense>
      </li>
    </ol>
  </div>
</template>
