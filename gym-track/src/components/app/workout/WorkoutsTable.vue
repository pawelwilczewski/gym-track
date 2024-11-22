<script setup lang="ts">
import Workout from '@/components/app/workout/Workout.vue';
import { toResult } from '@/scripts/errors/ResponseResult';
import { apiClient } from '@/scripts/http/Clients';
import { GetWorkoutResponse } from '@/scripts/schema/Types';
import { match, P } from 'ts-pattern';
import { Ref, ref } from 'vue';

const workouts: Ref<GetWorkoutResponse[] | undefined> = ref(undefined);

const update: () => Promise<void> = async () => {
  const response = await apiClient.get('/api/v1/workouts');
  match(toResult(response))
    .with({ type: 'success' }, () => {
      workouts.value = response.data;
    })
    .with({ type: 'empty' }, () => console.log('Unknown error encountered.'))
    .with({ type: 'message', message: P.select() }, message =>
      console.log(message)
    )
    .with({ type: 'validation', errors: P.select() }, errors => {
      errors.forEach(error => {
        console.log(error);
      });
    })
    .exhaustive();
};

defineExpose({
  update,
});

await update();
</script>

<template>
  <div v-if="workouts" class="flex flex-col gap-4">
    <Workout v-for="workout in workouts" :workout="workout" />
  </div>
</template>
