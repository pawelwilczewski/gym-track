<script setup lang="ts">
import Entity from '../../Entity.vue';
import { WorkoutExerciseKey } from '@/scripts/schema/Types';
import { useWorkoutExercise } from '@/composables/UseWorkoutExercise';
import ButtonDialog from '../../misc/ButtonDialog.vue';
import CreateWorkoutExerciseSet from './set/CreateWorkoutExerciseSet.vue';
import WorkoutExerciseSet from './set/WorkoutExerciseSet.vue';

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
    is="li"
    v-if="workoutExercise"
    @deleted="
      destroy();
      emit('deleted', props.exerciseKey);
    "
  >
    <div class="flex flex-col gap-6 py-8 px-4">
      <p>{{ workoutExercise.exerciseInfoId }}</p>

      <div>
        <h5 class="mb-2">Sets</h5>
        <ol class="list-decimal flex flex-col gap-6">
          <WorkoutExerciseSet
            v-for="key in workoutExercise.sets"
            :key="key.index"
            :exercise-set-key="key"
          />
        </ol>
      </div>

      <ButtonDialog dialog-title="Create Exercise Set">
        <template #button>Create Set</template>
        <template #dialog>
          <CreateWorkoutExerciseSet :workout-exercise-key="exerciseKey" />
        </template>
      </ButtonDialog>
    </div>
  </Entity>
</template>
