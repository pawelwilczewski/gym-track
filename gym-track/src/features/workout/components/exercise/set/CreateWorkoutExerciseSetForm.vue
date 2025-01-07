<script setup lang="ts">
import { useForm } from 'vee-validate';
import { createWorkoutExerciseSetSchema } from '@/features/workout/schemas/CreateWorkoutExerciseSetSchema';
import { WorkoutExerciseKey } from '@/features/workout/types/WorkoutTypes';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutExerciseSetForm from './WorkoutExerciseSetForm.vue';
import { useWorkoutExerciseSets } from '@/features/workout/stores/UseWorkoutExerciseSets';
import { createWorkoutExerciseSetSchemaToRequest } from '@/features/workout/schemas/CreateWorkoutExerciseSetSchema';
import { useWorkoutExercise } from '@/features/workout/composables/UseWorkoutExercise';
import { useWorkoutExerciseExerciseInfo } from '@/features/workout/composables/UseWorkoutExerciseExerciseInfo';

const { workoutExerciseKey } = defineProps<{
  workoutExerciseKey: WorkoutExerciseKey;
}>();

const sets = useWorkoutExerciseSets();
const { workoutExercise } = useWorkoutExercise(workoutExerciseKey);
const exerciseInfo = useWorkoutExerciseExerciseInfo(workoutExercise);

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
