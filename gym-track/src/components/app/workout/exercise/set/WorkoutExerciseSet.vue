<script setup lang="ts">
import Entity from '@/components/app/Entity.vue';
import { useWorkoutExerciseSet } from '@/composables/UseWorkoutExerciseSet';
import {
  Distance,
  Duration,
  ExerciseMetricType,
  GetExerciseInfoResponse,
  Weight,
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

const initialValues = computed(() => {
  if (!workoutExerciseSet.value) {
    return undefined;
  }

  const base = {
    reps: workoutExerciseSet.value.reps,
    metricType: workoutExerciseSet.value.metric.$type.toString(),
    distanceValue: undefined,
    distanceUnits: undefined,
    weightValue: undefined,
    weightUnits: undefined,
    time: undefined,
  };

  switch (workoutExerciseSet.value.metric.$type) {
    case ExerciseMetricType.Weight: {
      const weight = workoutExerciseSet.value.metric as Weight;
      return {
        ...base,
        weightUnits: weight.units.toString(),
        weightValue: weight.value,
      };
    }
    case ExerciseMetricType.Duration: {
      const duration = workoutExerciseSet.value.metric as Duration;
      return { ...base, time: duration.time };
    }
    case ExerciseMetricType.Distance: {
      const distance = workoutExerciseSet.value.metric as Distance;
      return {
        ...base,
        distanceUnits: distance.units.toString(),
        distanceValue: distance.value,
      };
    }
  }

  throw Error('Unreachable code');
});
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
        :initial-values="initialValues"
        @edited="
          update();
          closeDialog();
        "
      />
    </template>
  </Entity>
</template>
