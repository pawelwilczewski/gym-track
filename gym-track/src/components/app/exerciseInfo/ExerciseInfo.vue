<script setup lang="ts">
import { apiClient } from '@/scripts/http/Clients';
import { GetExerciseInfoResponse } from '@/scripts/schema/Types';

defineProps<{
  initialExerciseInfo: GetExerciseInfoResponse;
}>();
</script>

<template>
  <div
    class="mx-auto border border-border rounded-xl w-80 flex flex-col gap-6 p-8"
  >
    <h3>{{ initialExerciseInfo.name }}</h3>
    <p>{{ initialExerciseInfo.description }}</p>
    <div>{{ initialExerciseInfo.allowedMetricTypes }}</div>
    <picture>
      <source
        :srcset="`${apiClient.getUri()}/${initialExerciseInfo.thumbnailUrl}`"
      />
      <img />
    </picture>
    <ul>
      <li v-for="step in initialExerciseInfo.steps">
        {{ step.exerciseInfoId }}
        {{ step.index }}
      </li>
    </ul>
  </div>
</template>
