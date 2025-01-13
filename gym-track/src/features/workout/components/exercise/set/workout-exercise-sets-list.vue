<script setup lang="ts">
import WorkoutExerciseSet from '@/features/workout/components/exercise/set/workout-exercise-set.vue';
import { useWorkoutExercise } from '@/features/workout/composables/use-workout-exercise';
import { useWorkoutExerciseSetKeys } from '@/features/workout/composables/use-workout-exercise-set-keys';
import { useWorkoutExerciseSets } from '@/features/workout/stores/use-workout-exercise-sets';
import {
  hashWorkoutExerciseSetKey,
  WorkoutExerciseKey,
  WorkoutExerciseSetKey,
} from '@/features/workout/types/workout-types';
import { ref, watch } from 'vue';

const { workoutExerciseKey } = defineProps<{
  workoutExerciseKey: WorkoutExerciseKey;
}>();

const { workoutExercise } = useWorkoutExercise(workoutExerciseKey);
const setKeys = useWorkoutExerciseSetKeys(workoutExercise);
const sets = useWorkoutExerciseSets();

const sortedSetKeys = ref<WorkoutExerciseSetKey[]>([]);

watch(sets.all, async () => {
  if (
    setKeys.value.some(
      key => sets.all[hashWorkoutExerciseSetKey(key)] == undefined
    )
  ) {
    return;
  }

  sortedSetKeys.value = [...setKeys.value].sort(
    (a, b) =>
      sets.all[hashWorkoutExerciseSetKey(a)].displayOrder -
      sets.all[hashWorkoutExerciseSetKey(b)].displayOrder
  );
});

watch(setKeys, () => sets.fetchMultiple(setKeys.value), { immediate: true });
</script>

<template>
  <ol class="list-decimal flex flex-col gap-6">
    <WorkoutExerciseSet
      v-for="key in sortedSetKeys"
      :key="key.setIndex"
      :set-key="key"
    />
  </ol>
</template>
