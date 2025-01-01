<script setup lang="ts">
import ExerciseInfo from './ExerciseInfo.vue';
import { useExerciseInfos } from '@/app/stores/UseExerciseInfos';

const exerciseInfos = useExerciseInfos();
exerciseInfos.fetchAll();
</script>

<template>
  <div class="flex flex-col gap-4">
    <ExerciseInfo
      v-for="exerciseInfo in exerciseInfos.all"
      :key="exerciseInfo.id"
      :initial-exercise-info="exerciseInfo"
      @deleted="id => exerciseInfos.destroy(id)"
    />
  </div>
</template>
