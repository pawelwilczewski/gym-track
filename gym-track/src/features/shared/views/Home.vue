<script setup lang="ts">
import { Button } from '@/features/shared/components/ui/button';
import OneColumnLayout from '@/features/shared/layouts/one-column-layout.vue';
import { ChartLine, File, PersonStanding } from 'lucide-vue-next';
</script>

<template>
  <OneColumnLayout>
    <div class="flex flex-col items-center px-4 py-10 space-y-16 text-center">
      <div>
        <h1 class="text-4xl font-bold sm:text-5xl mb-8">Manage Your Workout</h1>
        <p class="text-lg max-w-2xl">
          With
          <span class="font-bold italic">Gym Track</span>
          you can easily create, use, and edit your workout routines. Take your
          fitness journey to the next level with personalized plans.
        </p>
      </div>

      <div
        class="flex flex-col sm:flex-row sm:space-x-4 space-y-4 sm:space-y-0"
      >
        <RouterLink to="/login">
          <Button size="lg">Get Started</Button>
        </RouterLink>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-8 w-full max-w-4xl">
        <div class="flex flex-col items-center p-6 rounded-lg shadow-md">
          <File class="w-16 h-16 mb-4" />
          <h3 class="text-xl font-semibold">Perfect your Routine</h3>
          <p class="text-center">
            Create personalized workout routines tailored to your goals.
          </p>
        </div>
        <div class="flex flex-col items-center p-6 rounded-lg shadow-md">
          <ChartLine class="w-16 h-16 mb-4" />
          <h3 class="text-xl font-semibold">Track Progress</h3>
          <p class="text-center">
            Monitor your progress and stay motivated every step of the way.
          </p>
        </div>
        <div class="flex flex-col items-center p-6 rounded-lg shadow-md">
          <PersonStanding class="w-16 h-16 mb-4" />
          <h3 class="text-xl font-semibold">Join Community</h3>
          <p class="text-center">
            Connect with fitness enthusiasts and share your journey.
          </p>
        </div>
      </div>
    </div>
  </OneColumnLayout>
</template>
