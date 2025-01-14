<script setup lang="ts">
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from '@/features/shared/components/ui/carousel';
import ExerciseInfo from './exercise-info.vue';
import { useExerciseInfos } from '@/features/exercise-info/stores/use-exercise-infos';

const exerciseInfos = useExerciseInfos();
exerciseInfos.fetchAll();
</script>

<template>
  <Carousel>
    <CarouselContent>
      <CarouselItem
        v-for="exerciseInfo in exerciseInfos.all"
        :key="exerciseInfo.id"
        class="max-w-lg"
      >
        <ExerciseInfo :id="exerciseInfo.id" />
      </CarouselItem>
    </CarouselContent>
    <CarouselPrevious />
    <CarouselNext />
  </Carousel>
</template>
