<script setup lang="ts">
import { Button } from '@/components/ui/button';
</script>

<template>
  <nav class="p-8">
    <div class="flex justify-between gap-24">
      <h1 class="text-4xl pb-3 px-3">
        <a href="/" class="font-bold">Gym Track</a>
      </h1>
      <div class="flex gap-6 flex-wrap">
        <div class="flex gap-2">
          <Button :variant="false ? 'ghost' : 'secondary'" asChild>
            <a href="/tracking">My Progress</a>
          </Button>
          <Button asChild variant="ghost">
            <a href="/workouts">Workouts</a>
          </Button>
          <Button asChild variant="ghost">
            <a href="/exercises">Exercises</a>
          </Button>
        </div>
        <div class="flex gap-2">
          <Button asChild>
            <a href="/logIn">Log In</a>
          </Button>
          <Button asChild variant="outline">
            <a href="/signUp">SignUp</a>
          </Button>
          <Button asChild variant="outline">
            <a href="/signUp">Sign Out</a>
          </Button>
        </div>
      </div>
    </div>
  </nav>
</template>
