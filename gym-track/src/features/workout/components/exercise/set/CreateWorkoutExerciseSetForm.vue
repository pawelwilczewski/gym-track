<script setup lang="ts">
import { useForm } from 'vee-validate';
import { createWorkoutExerciseSetSchema } from '@/features/workout/schemas/CreateWorkoutExerciseSetSchema';
import { GetExerciseInfoResponse } from '@/features/exerciseInfo/types/ExerciseInfoTypes';
import { WorkoutExerciseKey } from '@/features/workout/types/WorkoutTypes';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutExerciseSetForm from './WorkoutExerciseSetForm.vue';
import { useWorkoutExerciseSets } from '@/features/workout/stores/UseWorkoutExerciseSets';
import { createWorkoutExerciseSetSchemaToRequest } from '@/features/workout/schemas/CreateWorkoutExerciseSetSchema';

const { workoutExerciseKey, exerciseInfo } = defineProps<{
  workoutExerciseKey: WorkoutExerciseKey;
  exerciseInfo: GetExerciseInfoResponse | null;
}>();

const sets = useWorkoutExerciseSets();

const emit = defineEmits<{
  created: [];
}>();

const form = useForm({
  validationSchema: toTypedSchema(createWorkoutExerciseSetSchema),
});

const onSubmit = form.handleSubmit(async values => {
  await sets.create(
    workoutExerciseKey,
    createWorkoutExerciseSetSchemaToRequest(values),
    form
  );

  emit('created');
});
</script>

<template>
  <WorkoutExerciseSetForm
    :form="form"
    :exercise-info="exerciseInfo"
    submit-label="Create"
    :on-submit="onSubmit"
  />
</template>
