<script setup lang="ts">
import { useForm } from 'vee-validate';
import { editWorkoutSchema } from '@/app/schema/Schemas';
import { toTypedSchema } from '@vee-validate/zod';
import { z } from 'zod';
import { UUID } from 'crypto';
import WorkoutForm from './WorkoutForm.vue';
import { useWorkouts } from '@/app/stores/UseWorkouts';

const workouts = useWorkouts();

const props = defineProps<{
  workoutId: UUID;
  initialValues: z.infer<typeof editWorkoutSchema>;
}>();

const form = useForm({
  validationSchema: toTypedSchema(editWorkoutSchema),
});

const emit = defineEmits<{
  edited: [];
}>();

const handleSubmit = form.handleSubmit(async values => {
  workouts.update(props.workoutId, values, form);
  emit('edited');
});

if (props.initialValues) {
  form.setValues(props.initialValues, false);
}
</script>

<template>
  <WorkoutForm submit-label="Save" :on-submit="handleSubmit" />
</template>
