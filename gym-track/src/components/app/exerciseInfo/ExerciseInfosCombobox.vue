<script setup lang="ts">
import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { useExerciseInfos } from '@/composables/UseExerciseInfos';
import { UUID } from 'crypto';

const { exerciseInfos, update } = useExerciseInfos();

const emit = defineEmits<{
  selected: [UUID];
}>();

await update();
</script>

<template>
  <DropdownMenu>
    <DropdownMenuTrigger asChild>
      <Button variant="outline">Set Exercise</Button>
    </DropdownMenuTrigger>
    <DropdownMenuContent class="w-56">
      <DropdownMenuItem
        v-for="exerciseInfo in exerciseInfos"
        @click="emit('selected', exerciseInfo.id)"
        >{{ exerciseInfo.name }}</DropdownMenuItem
      >
    </DropdownMenuContent>
  </DropdownMenu>
</template>
