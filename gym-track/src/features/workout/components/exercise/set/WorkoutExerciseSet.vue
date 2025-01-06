<script setup lang="ts">
import Entity from '@/features/shared/components/Entity.vue';
import { useWorkoutExerciseSet } from '@/features/workout/composables/UseWorkoutExerciseSet';
import {
  GetExerciseInfoResponse,
  WorkoutExerciseSetKey,
} from '@/app/schema/Types';
import { computed } from 'vue';
import ExerciseMetric from '../../../../../features/workout/components/exercise/ExerciseMetric.vue';
import { Tally5 } from 'lucide-vue-next';
import EditWorkoutExerciseSetForm from './EditWorkoutExerciseSetForm.vue';

const props = defineProps<{
  exerciseSetKey: WorkoutExerciseSetKey;
  exerciseInfo: GetExerciseInfoResponse | undefined | null;
}>();

const { set, fetch, destroy } = useWorkoutExerciseSet(props.exerciseSetKey);
fetch();

const exerciseMetric = computed(() => set.value?.metric);
</script>

<template>
  <Entity is="li" v-if="set" @deleted="destroy()">
    <div class="flex flex-col gap-2 px-4">
      <span class="flex gap-1">
        <Tally5 class="w-4 h-4 my-auto" />
        {{ set.reps }}
      </span>
      <ExerciseMetric v-if="exerciseMetric" :exercise-metric="exerciseMetric" />
    </div>
    <template #edit="{ closeDialog }">
      <EditWorkoutExerciseSetForm
        :workout-exercise-set-key="exerciseSetKey"
        :exercise-info="exerciseInfo"
        @edited="closeDialog()"
      />
    </template>
  </Entity>
</template>
