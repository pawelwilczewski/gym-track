<script setup lang="ts">
import { useForm } from 'vee-validate';

import { UUID } from 'node:crypto';
import { toTypedSchema } from '@vee-validate/zod';
import { useTrackedWorkouts } from '@/features/tracked-workout/stores/use-tracked-workouts';
import { createTrackedWorkoutSchema } from '@/features/tracked-workout/schemas/create-tracked-workout-schema';
import TrackedWorkoutForm from '@/features/tracked-workout/components/tracked-workout-form.vue';

const trackedWorkouts = useTrackedWorkouts();

const emit = defineEmits<{
  created: [];
}>();

const form = useForm({
  validationSchema: toTypedSchema(createTrackedWorkoutSchema),
});

const onSubmit = form.handleSubmit(async values => {
  console.log('values');

  await trackedWorkouts.create(
    {
      workoutId: values.workoutId as UUID,
      performedAt: values.performedAt,
      duration: values.duration,
    },
    form
  );

  emit('created');
});
</script>

<template>
  <TrackedWorkoutForm :on-submit="onSubmit" submit-label="Create" />
</template>
