<script setup lang="ts">
import { Button } from '@/components/ui/button';
</script>

<template>
  <header class="mt-16">
    <div class="max-w-content-max flex justify-between gap-24">
      <h1 class="text-4xl pb-3 px-3">
        <a href="/">
          <b>Gym Track</b>
        </a>
      </h1>
      <div class="flex gap-2">
        <Button asChild>
          <a href="/tracking">My Progress</a>
        </Button>
        <Button asChild>
          <a href="/workouts">Workouts</a>
        </Button>
        <Button asChild>
          <a href="/exercises">Exercises</a>
        </Button>
        <Button asChild>
          <a href="/logIn">Log In</a>
        </Button>
        <Button asChild>
          <a href="/signUp">SignUp</a>
        </Button>
      </div>
    </div>
  </header>
  <main>
    <slot />
  </main>
  <footer>Goodbye!</footer>
</template>
