<script setup lang="ts">
import { useForm } from 'vee-validate';
import { createWorkoutExerciseSetSchema } from '@/app/schema/Schemas';
import {
  GetExerciseInfoResponse,
  WorkoutExerciseKey,
} from '@/app/schema/Types';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutExerciseSetForm from './WorkoutExerciseSetForm.vue';
import { useWorkoutExerciseSets } from '@/app/stores/UseWorkoutExerciseSets';
import { createWorkoutExerciseSetSchemaToRequest } from '@/app/schema/Converters';

const { workoutExerciseKey, exerciseInfo } = defineProps<{
  workoutExerciseKey: WorkoutExerciseKey;
  exerciseInfo: GetExerciseInfoResponse | undefined | null;
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
