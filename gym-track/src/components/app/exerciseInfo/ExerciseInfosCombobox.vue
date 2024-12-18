<script setup lang="ts">
import { Button } from '@/components/ui/button';
import {
  Command,
  CommandEmpty,
  CommandInput,
  CommandItem,
  CommandList,
} from '@/components/ui/command';
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/components/ui/popover';
import { useExerciseInfos } from '@/composables/UseExerciseInfos';
import { cn } from '@/lib/utils';
import { GetExerciseInfoResponse } from '@/app/schema/Types';
import { UUID } from 'crypto';
import { Check, ChevronsUpDown } from 'lucide-vue-next';
import { SelectEvent } from 'node_modules/radix-vue/dist/Combobox/ComboboxItem';
import { AcceptableValue } from 'node_modules/radix-vue/dist/shared/types';
import { ref } from 'vue';

const model = defineModel<UUID | undefined>();

const isOpen = ref(false);
const selectedValueRaw = ref('');
const { exerciseInfos, update } = useExerciseInfos();

function encodeSelectedValue(value: GetExerciseInfoResponse): string {
  return `${value.id}|${value.name}`;
}

function decodeSelectedValue(value: AcceptableValue): {
  id: UUID;
  name: string;
} {
  if (typeof value !== 'string') {
    throw Error(
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
      undefined
    )?.id;
  }
  isOpen.value = false;
}

update();
</script>

<template>
  <Popover v-if="exerciseInfos" v-model:open="isOpen">
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
            : 'Select exercise...'
        }}
        <ChevronsUpDown class="ml-2 h-4 w-4 shrink-0 opacity-50" />
      </Button>
    </PopoverTrigger>
    <PopoverContent class="w-[200px] p-0">
      <Command :filter-function="filterEntriesShown">
        <CommandInput class="h-9" placeholder="Search exercises..." />
        <CommandEmpty>No exercises found.</CommandEmpty>
        <CommandList>
          <CommandItem
            v-for="exerciseInfo in exerciseInfos"
            :key="exerciseInfo.id"
            :value="encodeSelectedValue(exerciseInfo)"
            @select="handleSelected"
          >
            {{ exerciseInfo.name }}
            <Check
              :class="
                cn(
                  'ml-auto h-4 w-4',
                  selectedValueRaw === encodeSelectedValue(exerciseInfo)
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
