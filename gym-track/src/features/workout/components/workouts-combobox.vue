<script setup lang="ts">
import { Button } from '@/features/shared/components/ui/button';
import {
  Command,
  CommandEmpty,
  CommandInput,
  CommandItem,
  CommandList,
} from '@/features/shared/components/ui/command';
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/features/shared/components/ui/popover';
import { cn } from '@/features/shared/utils/cn-utils';
import { UUID } from 'node:crypto';
import { Check, ChevronsUpDown } from 'lucide-vue-next';
import { SelectEvent } from 'node_modules/radix-vue/dist/Combobox/ComboboxItem';
import { AcceptableValue } from 'node_modules/radix-vue/dist/shared/types';
import { ref } from 'vue';
import { useWorkouts } from '@/features/workout/stores/use-workouts';
import { GetWorkoutResponse } from '@/features/workout/types/workout-types';

const model = defineModel<UUID | undefined>();

const isOpen = ref(false);
const selectedValueRaw = ref('');
const workouts = useWorkouts();
workouts.fetchAll();

function encodeSelectedValue(value: GetWorkoutResponse): string {
  return `${value.id}|${value.name}`;
}

function decodeSelectedValue(value: AcceptableValue): {
  id: UUID;
  name: string;
} {
  if (typeof value !== 'string') {
    throw new TypeError(
      'Unhandled decode value type, currently only supporting string!'
    );
  }

  const split = value.split('|', 2);
  return { id: split[0] as UUID, name: split[1] };
}

function decodeSelectedValueOrDefault<TAlternative>(
  value: string,
  alternative: TAlternative
): { id: UUID; name: string } | TAlternative {
  return value.length > 0 ? decodeSelectedValue(value) : alternative;
}

function filterEntriesShown(
  entries: AcceptableValue[],
  searchPhrase: string
): string[] {
  const searchPhraseLower = searchPhrase.toLowerCase();
  return entries
    .map(entry => entry as string)
    .filter(entry => {
      const itemValue = decodeSelectedValue(entry);
      return (
        itemValue.id.toLowerCase() === searchPhraseLower ||
        itemValue.name.toLowerCase().includes(searchPhraseLower)
      );
    });
}

function handleSelected(event: SelectEvent<AcceptableValue>): void {
  if (typeof event.detail.value === 'string') {
    selectedValueRaw.value = event.detail.value;
    model.value = decodeSelectedValueOrDefault(
      selectedValueRaw.value,
      // eslint-disable-next-line unicorn/no-useless-undefined
      undefined
    )?.id;
  }
  isOpen.value = false;
}
</script>

<template>
  <Popover v-if="workouts.all" v-model:open="isOpen">
    <PopoverTrigger as-child>
      <Button
        variant="outline"
        role="combobox"
        :aria-expanded="isOpen"
        class="w-full justify-between"
      >
        {{
          selectedValueRaw.length > 0
            ? decodeSelectedValue(selectedValueRaw).name
            : 'Select workout...'
        }}
        <ChevronsUpDown class="ml-2 h-4 w-4 shrink-0 opacity-50" />
      </Button>
    </PopoverTrigger>
    <PopoverContent class="w-[200px] p-0">
      <Command :filter-function="filterEntriesShown">
        <CommandInput class="h-9" placeholder="Search workouts..." />
        <CommandEmpty>No workouts found.</CommandEmpty>
        <CommandList>
          <CommandItem
            v-for="workout in workouts.all"
            :key="workout.id"
            :value="encodeSelectedValue(workout)"
            @select="handleSelected"
          >
            {{ workout.name }}
            <Check
              :class="
                cn(
                  'ml-auto h-4 w-4',
                  selectedValueRaw === encodeSelectedValue(workout)
                    ? 'opacity-100'
                    : 'opacity-0'
                )
              "
            />
          </CommandItem>
        </CommandList>
      </Command>
    </PopoverContent>
  </Popover>
</template>
