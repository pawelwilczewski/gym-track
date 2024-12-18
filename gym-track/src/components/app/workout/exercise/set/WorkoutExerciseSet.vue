<script setup lang="ts">
import Entity from '@/components/app/Entity.vue';
import { useWorkoutExerciseSet } from '@/composables/UseWorkoutExerciseSet';
import {
  GetExerciseInfoResponse,
  WorkoutExerciseSetKey,
} from '@/app/schema/Types';
import { computed } from 'vue';
import ExerciseMetric from '../ExerciseMetric.vue';
import { Tally5 } from 'lucide-vue-next';
import EditWorkoutExerciseSetForm from './EditWorkoutExerciseSetForm.vue';

const props = defineProps<{
  exerciseSetKey: WorkoutExerciseSetKey;
  exerciseInfo: GetExerciseInfoResponse | undefined | null;
}>();

const { workoutExerciseSet, update, destroy } = useWorkoutExerciseSet(
  props.exerciseSetKey,
  {
    immediate: true,
  }
);

const exerciseMetric = computed(() => workoutExerciseSet.value?.metric);

const emit = defineEmits<{
  deleted: [WorkoutExerciseSetKey];
}>();
</script>

<template>
  <Entity
    is="li"
    v-if="workoutExerciseSet"
    @deleted="
      destroy();
      emit('deleted', props.exerciseSetKey);
    "
  >
    <div class="flex flex-col gap-2 px-4">
      <span class="flex gap-1">
        <Tally5 class="w-4 h-4 my-auto" />
        {{ workoutExerciseSet.reps }}
      </span>
      <ExerciseMetric v-if="exerciseMetric" :exercise-metric="exerciseMetric" />
    </div>
    <template #edit="{ closeDialog }">
      <EditWorkoutExerciseSetForm
        :workout-exercise-set-key="exerciseSetKey"
        :exercise-info="exerciseInfo"
        :initial-values="{
          reps: workoutExerciseSet.reps,
          metricType: workoutExerciseSet.metric.$type,
        }"
        @edited="
          update();
          closeDialog();
        "
      />
    </template>
  </Entity>
</template>
