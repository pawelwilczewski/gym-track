<script setup lang="ts">
import Entity from '../../Entity.vue';
import { WorkoutExerciseKey } from '@/app/schema/Types';
import { useWorkoutExercise } from '@/composables/UseWorkoutExercise';
import ButtonDialog from '../../misc/ButtonDialog.vue';
import CreateWorkoutExerciseSet from './set/CreateWorkoutExerciseSet.vue';
import WorkoutExerciseSet from './set/WorkoutExerciseSet.vue';
import { useExerciseInfo } from '@/composables/UseExerciseInfo';
import { computed, watch } from 'vue';

const props = defineProps<{
  exerciseKey: WorkoutExerciseKey;
}>();

const { workoutExercise, update, destroy } = useWorkoutExercise(
  props.exerciseKey,
  {
    immediate: true,
  }
);

const { exerciseInfo, update: updateExerciseInfo } = useExerciseInfo(
  computed(() => workoutExercise.value?.exerciseInfoId)
);

watch(workoutExercise, () => updateExerciseInfo());

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
      <h5>{{ exerciseInfo?.name }}</h5>

      <div>
        <h6 class="mb-2">Sets</h6>
        <ol class="list-decimal flex flex-col gap-6">
          <WorkoutExerciseSet
            v-for="key in workoutExercise.sets"
            :key="key.index"
            :exercise-set-key="key"
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
          <CreateWorkoutExerciseSet
            :workout-exercise-key="exerciseKey"
            :exercise-info="exerciseInfo"
            @created="
              update();
              closeDialog();
            "
          />
        </template>
      </ButtonDialog>
    </div>
  </Entity>
</template>
