<script setup lang="ts">
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/features/shared/components/ui/form';
import Button from '@/features/shared/components/ui/button/Button.vue';
import ExerciseInfosCombobox from '@/features/exercise-info/components/exercise-infos-combobox.vue';

defineProps<{
  submitLabel: string;
  onSubmit: () => void;
}>();
</script>

<template>
  <form class="flex flex-col gap-6 mt-6" @submit="onSubmit">
    <FormField v-slot="{ componentField }" name="exerciseInfoId">
      <FormItem>
        <FormLabel class="text-lg !text-current">Exercise</FormLabel>
        <FormControl>
          <ExerciseInfosCombobox v-bind="componentField" />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>
    <Button class="mx-auto mt-4" type="submit">{{ submitLabel }}</Button>
  </form>
</template>
