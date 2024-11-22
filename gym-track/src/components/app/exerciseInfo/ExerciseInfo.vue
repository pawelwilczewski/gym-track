<script setup lang="ts">
import { apiClient } from '@/scripts/http/Clients';
import { GetExerciseInfoResponse } from '@/scripts/schema/Types';

defineProps<{
  exerciseInfo: GetExerciseInfoResponse;
}>();
</script>

<template>
  <div
    class="mx-auto border border-border rounded-xl w-80 flex flex-col gap-6 p-8"
  >
    <h3>{{ exerciseInfo.name }}</h3>
    <p>{{ exerciseInfo.description }}</p>
    <div>{{ exerciseInfo.allowedMetricTypes }}</div>
    <picture>
      <source :srcset="`${apiClient.getUri()}/${exerciseInfo.thumbnailUrl}`" />
      <img />
    </picture>
    <ul>
      <li v-for="step in exerciseInfo.steps">
        {{ step.exerciseInfoId }}
        {{ step.index }}
      </li>
    </ul>
  </div>
</template>
