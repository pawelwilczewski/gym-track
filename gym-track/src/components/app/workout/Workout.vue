<script setup lang="ts">
import Entity from '../Entity.vue';
import { GetWorkoutResponse } from '@/app/schema/Types';
import CreateWorkoutExercise from '@/components/app/workout/exercise/CreateWorkoutExercise.vue';
import WorkoutExercisesList from './exercise/WorkoutExercisesList.vue';
import ButtonDialog from '../misc/ButtonDialog.vue';
import { UUID } from 'crypto';
import { useWorkout } from '@/composables/UseWorkout';
import { useWorkoutExerciseKeys } from '@/composables/UseWorkoutExerciseKeys';

const props = defineProps<{
  initialWorkout: GetWorkoutResponse;
}>();

const { workout, update, destroy } = useWorkout(props.initialWorkout.id, {
  initialValue: props.initialWorkout,
});

const { exerciseKeys } = useWorkoutExerciseKeys(workout);

const emit = defineEmits<{ deleted: [UUID] }>();
</script>

<template>
  <Entity
    v-if="workout"
    class="card"
    @deleted="
      emit('deleted', workout.id);
      destroy();
    "
  >
    <h3>{{ workout.name }}</h3>
    <h4>Exercises</h4>
    <WorkoutExercisesList
      v-if="exerciseKeys"
      ref="exercisesList"
      :exercise-keys="exerciseKeys"
      @exercise-deleted="
        key => {
          exerciseKeys = exerciseKeys.filter(
            exerciseKey => exerciseKey !== key
          );
        }
      "
    />
    <ButtonDialog dialog-title="Create Workout Exercise">
      <template #button>Add Exercise</template>
      <template #dialog="{ closeDialog }">
        <CreateWorkoutExercise
          :workout-id="workout.id"
          @created="
            update();
            closeDialog();
          "
        />
      </template>
    </ButtonDialog>
  </Entity>
</template>
