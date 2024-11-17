<script setup lang="ts">
import { Button } from '@/components/ui/button';
import { isLoggedIn } from '@/scripts/auth/Auth';

const loggedIn = await isLoggedIn();
</script>

<template>
  <nav class="p-8">
    <div class="flex justify-between gap-24">
      <h1 class="pb-3 px-3">
        <a href="/" class="font-bold">Gym Track</a>
      </h1>
      <div class="flex gap-6 flex-wrap">
        <div v-if="loggedIn" class="flex gap-2">
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
            <a v-if="!loggedIn" href="/logIn">Log In</a>
          </Button>
          <Button v-if="!loggedIn" asChild variant="outline">
            <a href="/signUp">Sign Up</a>
          </Button>
          <Button v-if="loggedIn" asChild variant="outline">
            <a href="/logOut">Log Out</a>
          </Button>
        </div>
      </div>
    </div>
  </nav>
</template>
