<script setup lang="ts">
import { FormContext } from 'vee-validate';
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/features/shared/components/ui/form';
import Button from '@/features/shared/components/ui/button/Button.vue';
import Input from '@/features/shared/components/ui/input/Input.vue';
import Textarea from '@/features/shared/components/ui/textarea/Textarea.vue';
import Checkbox from '@/features/shared/components/ui/checkbox/Checkbox.vue';

defineProps<{
  form: FormContext;
  imageSection:
    | { type: 'upload' }
    | { type: 'replace'; currentImageUrl: string | null | undefined };
  submitLabel: string;
  onSubmit: () => void;
}>();
</script>

<template>
  <form class="flex flex-col gap-6 mt-6" @submit="onSubmit">
    <FormField v-slot="{ componentField }" name="description">
      <FormItem>
        <FormLabel class="text-lg !text-current">Description</FormLabel>
        <FormControl>
          <Textarea placeholder="Description" v-bind="componentField" />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>

    <FormField
      v-if="imageSection.type === 'replace'"
      v-slot="{ value, handleChange }"
      name="replaceImage"
    >
      <FormItem>
        <FormControl>
          <Checkbox :checked="value" @update:checked="handleChange" />
        </FormControl>
        <FormLabel class="ml-1 !text-current">Replace Image</FormLabel>
        <FormMessage />
      </FormItem>
    </FormField>

    <FormField
      v-if="
        imageSection.type === 'upload' ||
        (imageSection.type === 'replace' &&
          form.controlledValues.value.replaceImage)
      "
      v-slot="{ setValue }"
      name="image"
    >
      <FormItem>
        <FormLabel class="text-lg !text-current">Image</FormLabel>
        <FormControl>
          <template v-if="imageSection.type === 'replace'">
            <figure v-if="imageSection.currentImageUrl != null" class="py-2">
              <img
                :src="imageSection.currentImageUrl"
                alt="Image preview"
                class="step-image mx-auto"
              />
              <figcaption class="text-sm text-center mt-1">
                Previous image
              </figcaption>
            </figure>
            <p v-else class="text-sm py-2">No image previously set.</p>
          </template>

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
