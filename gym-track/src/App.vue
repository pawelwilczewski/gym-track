<script setup lang="ts">
import { Button } from '@/components/ui/button';

const test = (): void => alert('hello');
</script>

<template>
  <header>Hello World!</header>
  <Button @click="test">Click!</Button>
</template>
