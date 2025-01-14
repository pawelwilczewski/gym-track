<script setup lang="ts">
import CreateExerciseInfoForm from '@/features/exercise-info/components/create-exercise-info-form.vue';
import ExerciseInfosList from '@/features/exercise-info/components/exercise-infos-list.vue';
import ButtonDialog from '@/features/shared/components/button-dialog.vue';
import FullScreenLayout from '@/features/shared/layouts/full-screen-layout.vue';
</script>

<template>
  <FullScreenLayout>
    <section>
      <h1 class="mb-6">Your Exercises</h1>

      <div class="mb-6">
        <ButtonDialog dialog-title="Create New Exercise">
          <template #button>Create Exercise</template>
          <template #dialog="{ closeDialog }">
            <CreateExerciseInfoForm @created="closeDialog()" />
          </template>
        </ButtonDialog>
      </div>

      <ExerciseInfosList />
    </section>
  </FullScreenLayout>
</template>
