<script setup lang="ts">
import ExerciseInfo from './ExerciseInfo.vue';
import { useExerciseInfos } from '@/app/stores/UseExerciseInfos';

const { exerciseInfos, fetchExerciseInfos } = useExerciseInfos();
fetchExerciseInfos();
</script>

<template>
  <div class="flex flex-col gap-4">
    <ExerciseInfo
      v-for="exerciseInfo in exerciseInfos"
      :key="exerciseInfo.id"
      :initial-exercise-info="exerciseInfo"
      @deleted="
        id => {
          exerciseInfos = exerciseInfos.filter(
            exerciseInfo => exerciseInfo.id !== id
          );
        }
      "
    />
  </div>
</template>
