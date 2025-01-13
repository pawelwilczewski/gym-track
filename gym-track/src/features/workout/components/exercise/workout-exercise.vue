<script setup lang="ts">
import Entity from '@/features/shared/components/entity.vue';
import { WorkoutExerciseKey } from '@/features/workout/types/workout-types';
import { useWorkoutExercise } from '@/features/workout/composables/use-workout-exercise';
import ButtonDialog from '@/features/shared/components/button-dialog.vue';
import CreateWorkoutExerciseSetForm from '@/features/workout/components/exercise/set/create-workout-exercise-set-form.vue';
import { useWorkoutExerciseExerciseInfo } from '@/features/workout/composables/use-workout-exercise-exercise-info';
import WorkoutExerciseSetsList from '@/features/workout/components/exercise/set/workout-exercise-sets-list.vue';
import WorkoutExerciseReorderControls from '@/features/workout/components/exercise/workout-exercise-reorder-controls.vue';

const { exerciseKey } = defineProps<{ exerciseKey: WorkoutExerciseKey }>();

const { workoutExercise, destroy } = useWorkoutExercise(exerciseKey);
const exerciseInfo = useWorkoutExerciseExerciseInfo(workoutExercise);
</script>

<template>
  <Entity is="li" v-if="workoutExercise" :editable="false" @deleted="destroy()">
    <template #left>
      <WorkoutExerciseReorderControls :exercise-key="exerciseKey" />
    </template>

    <div class="flex flex-col gap-6 py-8 px-4">
      <h5>{{ exerciseInfo?.name }}</h5>

      <div>
        <h6 class="mb-2">Sets</h6>
        <WorkoutExerciseSetsList :workout-exercise-key="exerciseKey" />
      </div>

      <ButtonDialog dialog-title="Create Exercise Set">
        <template #button>Create Set</template>
        <template #dialog="{ closeDialog }">
          <CreateWorkoutExerciseSetForm
            :workout-exercise-key="exerciseKey"
            @created="closeDialog()"
          />
        </template>
      </ButtonDialog>
    </div>
  </Entity>
</template>
