<script setup lang="ts">
import { useForm } from 'vee-validate';

import { createWorkoutExerciseSchema } from '@/features/workout/schemas/create-workout-exercise-schema';
import { UUID } from 'node:crypto';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutExerciseForm from '@/features/workout/components/exercise/workout-exercise-form.vue';
import { useWorkoutExercises } from '@/features/workout/stores/use-workout-exercises';

const { workoutId } = defineProps<{
  workoutId: UUID;
}>();

const workoutExercises = useWorkoutExercises();

const emit = defineEmits<{
  created: [];
}>();

const form = useForm({
  validationSchema: toTypedSchema(createWorkoutExerciseSchema),
});

const onSubmit = form.handleSubmit(async values => {
  await workoutExercises.create(
    workoutId,
    {
      exerciseInfoId: values.exerciseInfoId as UUID,
    },
    form
  );

  emit('created');
});
</script>

<template>
  <WorkoutExerciseForm :on-submit="onSubmit" submit-label="Create" />
</template>
