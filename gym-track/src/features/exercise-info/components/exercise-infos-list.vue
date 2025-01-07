<script setup lang="ts">
import ExerciseInfo from './exercise-info.vue';
import { useExerciseInfos } from '@/features/exercise-info/stores/use-exercise-infos';

const exerciseInfos = useExerciseInfos();
exerciseInfos.fetchAll();
</script>

<template>
  <div class="flex flex-col gap-4">
    <ExerciseInfo
      v-for="exerciseInfo in exerciseInfos.all"
      :id="exerciseInfo.id"
      :key="exerciseInfo.id"
    />
  </div>
</template>
