<script setup lang="ts">
import Entity from '@/features/shared/components/entity.vue';
import { useWorkoutExerciseSet } from '@/features/workout/composables/use-workout-exercise-set';
import { WorkoutExerciseSetKey } from '@/features/workout/types/workout-types';
import { computed } from 'vue';
import ExerciseMetric from '../exercise-metric.vue';
import { Tally5 } from 'lucide-vue-next';
import EditWorkoutExerciseSetForm from './edit-workout-exercise-set-form.vue';
import { useWorkoutExerciseExerciseInfo } from '@/features/workout/composables/use-workout-exercise-exercise-info';
import { useWorkoutExercise } from '@/features/workout/composables/use-workout-exercise';
import WorkoutExerciseSetReorderControls from '@/features/workout/components/exercise/set/workout-exercise-set-reorder-controls.vue';

const { setKey } = defineProps<{
  setKey: WorkoutExerciseSetKey;
}>();

const { set, destroy } = useWorkoutExerciseSet(setKey);
const { workoutExercise } = useWorkoutExercise(setKey);
const exerciseInfo = useWorkoutExerciseExerciseInfo(workoutExercise);

const exerciseMetric = computed(() => set.value?.metric);
</script>

<template>
  <Entity is="li" v-if="set" @deleted="destroy()">
    <template #reorder>
      <WorkoutExerciseSetReorderControls :set-key="setKey" />
    </template>
    <div class="flex flex-col gap-2 px-4">
      <span class="flex gap-1">
        <Tally5 class="w-4 h-4 my-auto" />
        {{ set.reps }}
      </span>
      <ExerciseMetric v-if="exerciseMetric" :exercise-metric="exerciseMetric" />
    </div>
    <template #edit="{ closeDialog }">
      <EditWorkoutExerciseSetForm
        :workout-exercise-set-key="setKey"
        :exercise-info="exerciseInfo"
        @edited="closeDialog()"
      />
    </template>
  </Entity>
</template>
