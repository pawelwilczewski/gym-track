<script setup lang="ts">
import Entity from '../../Entity.vue';
import { WorkoutExerciseKey } from '@/app/schema/Types';
import { useWorkoutExercise } from '@/composables/UseWorkoutExercise';
import ButtonDialog from '../../misc/ButtonDialog.vue';
import WorkoutExerciseSet from './set/WorkoutExerciseSet.vue';
import { computed, watch } from 'vue';
import CreateWorkoutExerciseSetForm from './set/CreateWorkoutExerciseSetForm.vue';
import { useExerciseInfos } from '@/app/stores/UseExerciseInfos';

const { exerciseKey } = defineProps<{ exerciseKey: WorkoutExerciseKey }>();

const { workoutExercise, fetch, destroy } = useWorkoutExercise(exerciseKey);
fetch();

const exerciseInfos = useExerciseInfos();
watch(workoutExercise, () => {
  if (
    workoutExercise.value &&
    !exerciseInfos.all[workoutExercise.value.exerciseInfoId]
  ) {
    exerciseInfos.fetchById(workoutExercise.value.exerciseInfoId);
  }
});

const exerciseInfo = computed(() =>
  workoutExercise.value
    ? exerciseInfos.all[workoutExercise.value.exerciseInfoId]
    : null
);
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
            @deleted="
              key => {
                if (!workoutExercise) {
                  return;
                }
                workoutExercise.sets = workoutExercise.sets.filter(
                  exerciseKey => exerciseKey !== key
                );
              }
            "
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
