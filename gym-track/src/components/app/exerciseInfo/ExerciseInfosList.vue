<script setup lang="ts">
import ExerciseInfo from './ExerciseInfo.vue';
import { useExerciseInfos } from '@/app/stores/UseExerciseInfos';

const store = useExerciseInfos();
store.fetchExerciseInfos();
</script>

<template>
  <div class="flex flex-col gap-4">
    <ExerciseInfo
      v-for="exerciseInfo in store.exerciseInfos"
      :key="exerciseInfo.id"
      :initial-exercise-info="exerciseInfo"
      @deleted="
        id => {
          // TODO Pawel: this should be abstracted in store
          store.exerciseInfos = store.exerciseInfos.filter(
            exerciseInfo => exerciseInfo.id !== id
          );
        }
      "
    />
  </div>
</template>
