<script setup lang="ts">
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/features/shared/components/ui/form';
import Button from '@/features/shared/components/ui/button/Button.vue';
import WorkoutsCombobox from '@/features/workout/components/workouts-combobox.vue';
import { cn } from '@/features/shared/utils/cn-utils';
import PopoverContent from '@/features/shared/components/ui/popover/PopoverContent.vue';
import Popover from '@/features/shared/components/ui/popover/Popover.vue';
import PopoverTrigger from '@/features/shared/components/ui/popover/PopoverTrigger.vue';
import { CalendarIcon } from 'lucide-vue-next';
import Input from '@/features/shared/components/ui/input/Input.vue';
import { formatDateTime } from '@/features/shared/utils/formatters';
import Calendar from '@/features/shared/components/ui/v-calendar/Calendar.vue';

defineProps<{
  submitLabel: string;
  onSubmit: () => void;
  includeWorkoutIdField: boolean;
}>();
</script>

<template>
  <form class="flex flex-col gap-6 mt-6" @submit="onSubmit">
    <FormField
      v-if="includeWorkoutIdField"
      v-slot="{ componentField }"
      name="workoutId"
    >
      <FormItem>
        <FormLabel class="text-lg !text-current">Workout</FormLabel>
        <FormControl>
          <WorkoutsCombobox v-bind="componentField" />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>

    <FormField v-slot="{ value, setValue }" name="performedAt">
      <FormItem>
        <FormLabel class="text-lg !text-current">Performed at</FormLabel>
        <FormControl>
          <Popover>
            <PopoverTrigger as-child>
              <Button
                variant="outline"
                :class="
                  cn(
                    'flex w-[280px] justify-start text-left font-normal',
                    !value && 'text-muted-foreground'
                  )
                "
              >
                <CalendarIcon class="mr-2 h-4 w-4" />
                <span v-if="value">{{ formatDateTime(value) }}</span>
                <span v-else>Date and time</span>
              </Button>
            </PopoverTrigger>
            <PopoverContent class="w-auto p-0">
              <Calendar
                initial-focus
                mode="datetime"
                @update:model-value="setValue"
              />
            </PopoverContent>
          </Popover>
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>

    <FormField v-slot="{ componentField }" name="duration">
      <FormItem>
        <FormLabel class="text-lg !text-current">Duration</FormLabel>
        <FormControl>
          <Input type="time" step="1" v-bind="componentField" />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>

    <Button class="mx-auto px-8 mt-4" type="submit">{{ submitLabel }}</Button>
  </form>
</template>
