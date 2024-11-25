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
import { GetExerciseInfoResponse } from '@/scripts/schema/Types';
import { UUID } from 'crypto';
import { Check, ChevronsUpDown } from 'lucide-vue-next';
import { Ref, ref } from 'vue';

const emit = defineEmits<{
  selected: [UUID];
}>();

const { exerciseInfos, update } = useExerciseInfos();

function encodeSearchValue(value: GetExerciseInfoResponse): string {
  return `${value.id}|${value.name}`;
}

function decodeSearchValue(value: string): { id: UUID; name: string } {
  const split = value.split('|', 2);
  return { id: split[0] as UUID, name: split[1] };
}

function filter(items: any[], searchPhrase: string): string[] {
  return items.filter(item => {
    console.log('filtering');
    const itemValue = decodeSearchValue(item);
    return (
      itemValue.id === searchPhrase ||
      itemValue.name.toLowerCase().includes(searchPhrase.toLowerCase())
    );
  });
}

const open = ref(false);
const selectedValueRaw = ref('');
const selectedValue: Ref<UUID | undefined> = ref(undefined);

defineExpose({
  selectedValue,
});

await update();
</script>

<template>
  <Popover v-model:open="open" v-if="exerciseInfos">
    <PopoverTrigger as-child>
      <Button
        variant="outline"
        role="combobox"
        :aria-expanded="open"
        class="w-[200px] justify-between"
      >
        {{
          selectedValueRaw
            ? exerciseInfos.find(
                exerciseInfo =>
                  encodeSearchValue(exerciseInfo) === selectedValueRaw
              )?.name
            : 'Select exercise...'
        }}
        <ChevronsUpDown class="ml-2 h-4 w-4 shrink-0 opacity-50" />
      </Button>
    </PopoverTrigger>
    <PopoverContent class="w-[200px] p-0">
      <Command :filterFunction="filter">
        <CommandInput class="h-9" placeholder="Search exercises..." />
        <CommandEmpty>No exercises found.</CommandEmpty>
        <CommandList>
          <CommandItem
            v-for="exerciseInfo in exerciseInfos"
            :key="exerciseInfo.id"
            :value="encodeSearchValue(exerciseInfo)"
            @select="
              event => {
                if (typeof event.detail.value === 'string') {
                  selectedValueRaw = event.detail.value;
                  selectedValue = decodeSearchValue(selectedValueRaw).id;
                  emit('selected', selectedValue);
                }
                open = false;
              }
            "
          >
            {{ exerciseInfo.name }}
            <Check
              :class="
                cn(
                  'ml-auto h-4 w-4',
                  selectedValueRaw === encodeSearchValue(exerciseInfo)
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
