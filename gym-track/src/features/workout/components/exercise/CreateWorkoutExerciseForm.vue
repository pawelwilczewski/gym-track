<script setup lang="ts">
import { useForm } from 'vee-validate';

import { createWorkoutExerciseSchema } from '@/features/workout/schemas/CreateWorkoutExerciseSchema';
import { UUID } from 'crypto';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutExerciseForm from '@/features/workout/components/exercise/WorkoutExerciseForm.vue';
import { useWorkoutExercises } from '@/features/workout/stores/UseWorkoutExercises';

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
