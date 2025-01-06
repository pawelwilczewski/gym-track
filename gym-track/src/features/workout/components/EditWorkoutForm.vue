<script setup lang="ts">
import { useForm } from 'vee-validate';
import { editWorkoutSchema } from '@/features/workout/schemas/EditWorkoutSchema';
import { toTypedSchema } from '@vee-validate/zod';
import { UUID } from 'crypto';
import WorkoutForm from './WorkoutForm.vue';
import { useWorkout } from '@/features/workout/composables/UseWorkout';

const { workoutId } = defineProps<{ workoutId: UUID }>();

const { workout, update } = useWorkout(workoutId);

const form = useForm({
  validationSchema: toTypedSchema(editWorkoutSchema),
});

const emit = defineEmits<{
  edited: [];
}>();

const handleSubmit = form.handleSubmit(async values => {
  update(values, form);
  emit('edited');
});

if (workout) {
  form.setValues({ name: workout.value.name }, false);
}
</script>

<template>
  <WorkoutForm submit-label="Save" :on-submit="handleSubmit" />
</template>
