<script setup lang="ts">
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import Button from '@/components/ui/button/Button.vue';
import Input from '@/components/ui/input/Input.vue';
import Textarea from '@/components/ui/textarea/Textarea.vue';
import ExerciseMetricTypeToggleGroup from './ExerciseMetricTypeToggleGroup.vue';
import Checkbox from '@/components/ui/checkbox/Checkbox.vue';
import { FormContext } from 'vee-validate';

defineProps<{
  form: FormContext;
  thumbnailImageSection: 'upload' | 'replace';
  currentThumbnailImageUrl?: string | null | undefined;
  submitLabel: string;
  onSubmit: () => void;
}>();
</script>

<template>
  <form class="flex flex-col gap-6 mt-6" @submit="onSubmit">
    <FormField v-slot="{ componentField }" name="name">
      <FormItem>
        <FormLabel class="text-lg !text-current">Name</FormLabel>
        <FormControl>
          <Input placeholder="Name" v-bind="componentField" />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>

    <FormField v-slot="{ componentField }" name="description">
      <FormItem>
        <FormLabel class="text-lg !text-current">Description</FormLabel>
        <FormControl>
          <Textarea placeholder="Description" v-bind="componentField" />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>

    <FormField v-slot="{ componentField }" name="allowedMetricTypes">
      <FormItem>
        <FormLabel class="text-lg !text-current">
          Allowed Metric Types
        </FormLabel>
        <FormControl>
          <ExerciseMetricTypeToggleGroup
            toggle-type="multiple"
            v-bind="componentField"
          />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>

    <FormField
      v-if="thumbnailImageSection === 'replace'"
      v-slot="{ value, handleChange }"
      name="replaceThumbnailImage"
    >
      <FormItem>
        <FormControl>
          <Checkbox :checked="value" @update:checked="handleChange" />
        </FormControl>
        <FormLabel class="ml-1 !text-current">
          Replace Thumbnail Image
        </FormLabel>
        <FormMessage />
      </FormItem>
    </FormField>

    <FormField
      v-if="
        thumbnailImageSection === 'upload' ||
        (thumbnailImageSection === 'replace' &&
          form.controlledValues.value.replaceThumbnailImage)
      "
      v-slot="{ setValue }"
      name="thumbnailImage"
    >
      <FormItem>
        <FormLabel class="text-lg !text-current">Thumbnail Image</FormLabel>
        <FormControl>
          <figure v-if="currentThumbnailImageUrl != null">
            <img
              :src="currentThumbnailImageUrl"
              alt="Thumbnail preview"
              class="thumbnail-image mx-auto"
            />
            <figcaption class="text-sm text-center mt-1">
              Previous thumbnail image
            </figcaption>
          </figure>
          <p v-else-if="currentThumbnailImageUrl === null">
            No thumbnail image currently set.
          </p>

          <Input
            type="file"
            accept="image/jpeg,image/png,image/gif"
            :onchange="
              (event: Event) => {
                const input = event?.currentTarget as HTMLInputElement;
                setValue(input?.files?.[0]);
              }
            "
          />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>
    <Button class="mx-auto px-8 mt-4" type="submit">{{ submitLabel }}</Button>
  </form>
</template>
