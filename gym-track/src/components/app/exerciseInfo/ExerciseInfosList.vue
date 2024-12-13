<script setup lang="ts">
import ExerciseInfo from './ExerciseInfo.vue';
import { useExerciseInfos } from '@/composables/UseExerciseInfos';

const { exerciseInfos, update } = useExerciseInfos({ immediate: true });

defineExpose({
  update,
});
</script>

<template>
  <div v-if="exerciseInfos" class="flex flex-col gap-4">
    <ExerciseInfo
      v-for="exerciseInfo in exerciseInfos"
      :initialExerciseInfo="exerciseInfo"
    />
  </div>
</template>
