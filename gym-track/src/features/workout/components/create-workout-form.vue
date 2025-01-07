<script setup lang="ts">
import { useForm } from 'vee-validate';
import { createWorkoutSchema } from '@/features/workout/schemas/create-workout-schema';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutForm from './workout-form.vue';
import { useWorkouts } from '@/features/workout/stores/use-workouts';

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
