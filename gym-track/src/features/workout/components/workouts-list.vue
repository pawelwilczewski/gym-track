<script setup lang="ts">
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from '@/features/shared/components/ui/carousel';
import Workout from '@/features/workout/components/workout.vue';
import { useWorkouts } from '@/features/workout/stores/use-workouts';

const workouts = useWorkouts();
workouts.fetchAll();
</script>

<template>
  <Carousel>
    <CarouselContent>
      <CarouselItem
        v-for="workout in workouts.allSorted"
        :key="workout.id"
        class="max-w-lg"
      >
        <Workout :id="workout.id" />
      </CarouselItem>
    </CarouselContent>
    <CarouselPrevious />
    <CarouselNext />
  </Carousel>
</template>
