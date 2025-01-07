<script setup lang="ts">
import Entity from '@/features/shared/components/Entity.vue';
import { WorkoutExerciseKey } from '@/features/workout/types/WorkoutTypes';
import { useWorkoutExercise } from '@/features/workout/composables/UseWorkoutExercise';
import ButtonDialog from '@/features/shared/components/ButtonDialog.vue';
import WorkoutExerciseSet from '@/features/workout/components/exercise/set/WorkoutExerciseSet.vue';
import CreateWorkoutExerciseSetForm from '@/features/workout/components/exercise/set/CreateWorkoutExerciseSetForm.vue';
import { useWorkoutExerciseExerciseInfo } from '@/features/workout/composables/UseWorkoutExerciseExerciseInfo';

const { exerciseKey } = defineProps<{ exerciseKey: WorkoutExerciseKey }>();

const { workoutExercise, destroy } = useWorkoutExercise(exerciseKey);
const exerciseInfo = useWorkoutExerciseExerciseInfo(workoutExercise);
</script>

<template>
  <Entity is="li" v-if="workoutExercise" :editable="false" @deleted="destroy()">
    <div class="flex flex-col gap-6 py-8 px-4">
      <h5>{{ exerciseInfo?.name }}</h5>

      <div>
        <h6 class="mb-2">Sets</h6>
        <ol class="list-decimal flex flex-col gap-6">
          <WorkoutExerciseSet
            v-for="key in workoutExercise.sets"
            :key="key.index"
            :exercise-set-key="key"
            :exercise-info="exerciseInfo"
          />
        </ol>
      </div>

      <ButtonDialog dialog-title="Create Exercise Set">
        <template #button>Create Set</template>
        <template #dialog="{ closeDialog }">
          <CreateWorkoutExerciseSetForm
            :workout-exercise-key="exerciseKey"
            :exercise-info="exerciseInfo"
            @created="closeDialog()"
          />
        </template>
      </ButtonDialog>
    </div>
  </Entity>
</template>
