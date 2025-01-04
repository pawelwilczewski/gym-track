<script setup lang="ts">
import Entity from '../Entity.vue';
import CreateWorkoutExerciseForm from '@/components/app/workout/exercise/CreateWorkoutExerciseForm.vue';
import WorkoutExercisesList from './exercise/WorkoutExercisesList.vue';
import ButtonDialog from '../misc/ButtonDialog.vue';
import { UUID } from 'crypto';
import { useWorkoutExerciseKeys } from '@/composables/UseWorkoutExerciseKeys';
import EditWorkoutForm from './EditWorkoutForm.vue';
import { useWorkout } from '@/composables/UseWorkout';

const { id } = defineProps<{ id: UUID }>();

const { workout, destroy } = useWorkout(id);

const { exerciseKeys } = useWorkoutExerciseKeys(workout);
</script>

<template>
  <Entity v-if="workout" class="card" @deleted="destroy()">
    <h3>{{ workout.name }}</h3>
    <h4>Exercises</h4>
    <WorkoutExercisesList v-if="exerciseKeys" :exercise-keys="exerciseKeys" />
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
