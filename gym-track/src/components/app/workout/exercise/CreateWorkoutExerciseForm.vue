<script setup lang="ts">
import { useForm } from 'vee-validate';

import { createWorkoutExerciseSchema } from '@/app/schema/Schemas';
import { UUID } from 'crypto';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutExerciseForm from '@/components/app/workout/exercise/WorkoutExerciseForm.vue';
import { useWorkoutExercises } from '@/app/stores/UseWorkoutExercises';

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
