<script setup lang="ts">
import ExerciseInfo from './ExerciseInfo.vue';
import { useExerciseInfos } from '@/composables/UseExerciseInfos';

const { exerciseInfos, update } = useExerciseInfos();
update();
</script>

<template>
  <div v-if="exerciseInfos" class="flex flex-col gap-4">
    <ExerciseInfo
      v-for="exerciseInfo in exerciseInfos"
      :exerciseInfo="exerciseInfo"
    />
  </div>
</template>
