<script setup lang="ts">
import { useForm } from 'vee-validate';

import { UUID } from 'node:crypto';
import { toTypedSchema } from '@vee-validate/zod';
import { useTrackedWorkouts } from '@/features/tracked-workout/stores/use-tracked-workouts';
import TrackedWorkoutForm from '@/features/tracked-workout/components/tracked-workout-form.vue';
import { useTrackedWorkout } from '@/features/tracked-workout/composables/use-tracked-workout';
import { editTrackedWorkoutSchema } from '@/features/tracked-workout/schemas/edit-tracked-workout-schema';

const { trackedWorkoutId } = defineProps<{ trackedWorkoutId: UUID }>();

const trackedWorkouts = useTrackedWorkouts();

const emit = defineEmits<{
  edited: [];
}>();

const form = useForm({
  validationSchema: toTypedSchema(editTrackedWorkoutSchema),
});

const onSubmit = form.handleSubmit(async values => {
  console.log('submitted');
  await trackedWorkouts.update(
    trackedWorkoutId,
    {
      performedAt: values.performedAt,
      duration: values.duration,
    },
    form
  );

  emit('edited');
});

const { trackedWorkout } = useTrackedWorkout(trackedWorkoutId);
if (trackedWorkout.value) {
  form.setValues({
    performedAt: new Date(trackedWorkout.value.performedAt),
    duration: trackedWorkout.value.duration,
  });
}
</script>

<template>
  <TrackedWorkoutForm
    :include-workout-id-field="false"
    :on-submit="onSubmit"
    submit-label="Save"
  />
</template>
