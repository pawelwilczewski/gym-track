<script setup lang="ts">
import { useForm } from 'vee-validate';
import { createWorkoutSchema } from '@/features/workout/schemas/CreateWorkoutSchema';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutForm from './WorkoutForm.vue';
import { useWorkouts } from '@/features/workout/stores/UseWorkouts';

const workouts = useWorkouts();

const form = useForm({
  validationSchema: toTypedSchema(createWorkoutSchema),
});

const emit = defineEmits<{
  created: [];
}>();

const handleSubmit = form.handleSubmit(async values => {
  workouts.create(values, form);
  emit('created');
});
</script>

<template>
  <WorkoutForm :on-submit="handleSubmit" submit-label="Create" />
</template>
