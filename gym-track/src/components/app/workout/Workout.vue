<script setup lang="ts">
import Button from '@/components/ui/button/Button.vue';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { apiClient } from '@/scripts/http/Clients';
import { GetWorkoutResponse } from '@/scripts/schema/Types';
import { Ref, ref } from 'vue';
import WorkoutExercises from '@/components/app/workout/exercise/WorkoutExercises.vue';
import Dialog from '@/components/ui/dialog/Dialog.vue';
import DialogTrigger from '@/components/ui/dialog/DialogTrigger.vue';
import DialogContent from '@/components/ui/dialog/DialogContent.vue';
import CreateWorkoutExercise from '@/components/app/workout/exercise/CreateWorkoutExercise.vue';
import { DialogTitle } from 'radix-vue';

const props = defineProps<{
  workout: GetWorkoutResponse;
}>();

async function deleteWorkout(): Promise<void> {
  if (!workout) {
    return;
  }
  const response = await apiClient.delete(
    `/api/v1/workouts/${props.workout.id}`
  );
  if (
    ErrorHandler.forResponse(response).handleFully(toastErrorHandler).wasError()
  ) {
    return;
  }
  workout.value = undefined;
}

const workout: Ref<GetWorkoutResponse | undefined> = ref(props.workout);
</script>

<template>
  <div
    v-if="workout"
    class="mx-auto border border-border rounded-xl w-80 flex flex-col gap-6 p-8"
  >
    <h3>{{ workout.name }}</h3>
    <Button @click="deleteWorkout">Delete</Button>
    <h4>Exercises</h4>
    <WorkoutExercises :workout="workout" />
    <Dialog>
      <DialogTrigger>
        <Button variant="outline">Add Exercise</Button>
      </DialogTrigger>
      <DialogContent>
        <DialogTitle>Create Workout Exercise</DialogTitle>
        <CreateWorkoutExercise :workout="workout" />
      </DialogContent>
    </Dialog>
  </div>
</template>
