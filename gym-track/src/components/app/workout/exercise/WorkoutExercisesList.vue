<script setup lang="ts">
import { WorkoutExerciseKey } from '@/scripts/schema/Types';
import WorkoutExercise from '@/components/app/workout/exercise/WorkoutExercise.vue';

defineProps<{
  exerciseKeys: WorkoutExerciseKey[];
}>();

const emit = defineEmits<{
  exerciseDeleted: [WorkoutExerciseKey];
}>();
</script>

<template>
  <ol class="list-decimal flex flex-col gap-4">
    <WorkoutExercise
      v-for="key in exerciseKeys"
      :key="key.index"
      :exercise-key="key"
      @deleted="emit('exerciseDeleted', key)"
    />
  </ol>
</template>
