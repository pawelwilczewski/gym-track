<script setup lang="ts">
import { useForm } from 'vee-validate';
import { createWorkoutExerciseSetSchema } from '@/features/workout/schemas/create-workout-exercise-set-schema';
import { WorkoutExerciseKey } from '@/features/workout/types/workout-types';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutExerciseSetForm from './workout-exercise-set-form.vue';
import { useWorkoutExerciseSets } from '@/features/workout/stores/use-workout-exercise-sets';
import { createWorkoutExerciseSetSchemaToRequest } from '@/features/workout/schemas/create-workout-exercise-set-schema';
import { useWorkoutExercise } from '@/features/workout/composables/use-workout-exercise';
import { useWorkoutExerciseExerciseInfo } from '@/features/workout/composables/use-workout-exercise-exercise-info';

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
