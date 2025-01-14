<script setup lang="ts">
import Entity from '@/features/shared/components/entity.vue';
import CreateWorkoutExerciseForm from '@/features/workout/components/exercise/create-workout-exercise-form.vue';
import WorkoutExercisesList from './exercise/workout-exercises-list.vue';
import ButtonDialog from '@/features/shared/components/button-dialog.vue';
import { UUID } from 'node:crypto';
import EditWorkoutForm from './edit-workout-form.vue';
import { useWorkout } from '@/features/workout/composables/use-workout';

const { id } = defineProps<{ id: UUID }>();

const { workout, destroy } = useWorkout(id);
</script>

<template>
  <Entity
    v-if="workout"
    class="card h-[600px] overflow-y-auto"
    @deleted="destroy()"
  >
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
