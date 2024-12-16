<script setup lang="ts">
import Entity from '../../Entity.vue';
import { WorkoutExerciseKey } from '@/scripts/schema/Types';
import { useWorkoutExercise } from '@/composables/UseWorkoutExercise';
import ButtonDialog from '../../misc/ButtonDialog.vue';
import CreateWorkoutExerciseSet from './set/CreateWorkoutExerciseSet.vue';

const props = defineProps<{
  exerciseKey: WorkoutExerciseKey;
}>();

const { workoutExercise, destroy } = useWorkoutExercise(props.exerciseKey, {
  immediate: true,
});

const emit = defineEmits<{
  deleted: [WorkoutExerciseKey];
}>();
</script>

<template>
  <Entity
    v-if="workoutExercise"
    class="flex flex-col gap-6 p-8"
    @deleted="
      destroy();
      emit('deleted', props.exerciseKey);
    "
  >
    <h4>{{ workoutExercise.index }}</h4>
    <p>{{ workoutExercise.exerciseInfoId }}</p>
    <div v-for="set in workoutExercise.sets">{{ set.workoutId }}</div>

    <ButtonDialog dialog-title="Create Exercise Set">
      <template #button>Create Set</template>
      <template #dialog>
        <CreateWorkoutExerciseSet :workout-exercise-key="exerciseKey" />
      </template>
    </ButtonDialog>
  </Entity>
</template>
