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
import { SelectEvent } from 'node_modules/radix-vue/dist/Combobox/ComboboxItem';
import { AcceptableValue } from 'node_modules/radix-vue/dist/shared/types';
import { computed, ref } from 'vue';

const emit = defineEmits<{
  selected: [UUID];
}>();

const model = defineModel();

const isOpen = ref(false);
const selectedValueRaw = ref('');
const selectedValue = computed(() =>
  selectedValueRaw.value.length > 0
    ? decodeSearchValue(selectedValueRaw.value).id
    : undefined
);

const { exerciseInfos, update: updateEntries } = useExerciseInfos();

function encodeSearchValue(value: GetExerciseInfoResponse): string {
  return `${value.id}|${value.name}`;
}

function decodeSearchValue(value: string): { id: UUID; name: string } {
  const split = value.split('|', 2);
  return { id: split[0] as UUID, name: split[1] };
}

function filter(items: any[], searchPhrase: string): string[] {
  const searchPhraseLower = searchPhrase.toLowerCase();
  return items.filter(item => {
    const itemValue = decodeSearchValue(item);
    return (
      itemValue.id.toLowerCase() === searchPhraseLower ||
      itemValue.name.toLowerCase().includes(searchPhraseLower)
    );
  });
}

function handleSelected(event: SelectEvent<AcceptableValue>): void {
  if (typeof event.detail.value === 'string') {
    selectedValueRaw.value = event.detail.value;
    model.value = selectedValue.value;
    emit('selected', selectedValue.value!);
  }
  isOpen.value = false;
}

defineExpose({
  selectedValue,
});

await updateEntries();
</script>

<template>
  <Popover v-model:open="isOpen" v-if="exerciseInfos">
    <PopoverTrigger as-child>
      <Button
        variant="outline"
        role="combobox"
        :aria-expanded="isOpen"
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
            @select="handleSelected"
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
