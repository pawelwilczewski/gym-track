<script setup lang="ts">
import ExerciseInfo from './ExerciseInfo.vue';
import { useExerciseInfos } from '@/features/exerciseInfo/stores/UseExerciseInfos';

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
