<script setup lang="ts">
import Entity from '@/features/shared/components/Entity.vue';
import CreateWorkoutExerciseForm from '@/features/workout/components/exercise/CreateWorkoutExerciseForm.vue';
import WorkoutExercisesList from './exercise/WorkoutExercisesList.vue';
import ButtonDialog from '@/features/shared/components/ButtonDialog.vue';
import { UUID } from 'crypto';
import EditWorkoutForm from './EditWorkoutForm.vue';
import { useWorkout } from '@/features/workout/composables/UseWorkout';

const { id } = defineProps<{ id: UUID }>();

const { workout, destroy } = useWorkout(id);
</script>

<template>
  <Entity v-if="workout" class="card" @deleted="destroy()">
    <h3>{{ workout.name }}</h3>
    <h4>Exercises</h4>
    <WorkoutExercisesList :workout-id="id" />
    <ButtonDialog dialog-title="Create Workout Exercise">
      <template #button>Add Exercise</template>
      <template #dialog="{ closeDialog }">
        <CreateWorkoutExerciseForm
          :workout-id="workout.id"
          @created="closeDialog()"
        />
      </template>
    </ButtonDialog>
    <template #edit="{ closeDialog }">
      <EditWorkoutForm
        :workout-id="workout.id"
        :initial-values="{ name: workout.name }"
        @edited="closeDialog()"
      />
    </template>
  </Entity>
</template>
