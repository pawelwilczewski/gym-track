<script setup lang="ts">
import { Button } from '@/components/ui/button';
import { isLoggedIn } from '@/scripts/auth/Auth';

const loggedIn = await isLoggedIn();
</script>

<template>
  <nav class="p-8">
    <div class="flex justify-between gap-24">
      <h1 class="pb-3 px-3">
        <RouterLink to="/" class="font-bold">Gym Track</RouterLink>
      </h1>
      <div class="flex gap-6 flex-wrap">
        <div v-if="loggedIn" class="flex gap-2">
          <Button :variant="false ? 'ghost' : 'secondary'" asChild>
            <RouterLink to="/tracking">My Progress</RouterLink>
          </Button>
          <Button asChild variant="ghost">
            <RouterLink to="/workouts">Workouts</RouterLink>
          </Button>
          <Button asChild variant="ghost">
            <RouterLink to="/exercises">Exercises</RouterLink>
          </Button>
        </div>
        <div class="flex gap-2">
          <Button asChild>
            <RouterLink v-if="!loggedIn" to="/logIn">Log In</RouterLink>
          </Button>
          <Button v-if="!loggedIn" asChild variant="outline">
            <RouterLink to="/signUp">Sign Up</RouterLink>
          </Button>
          <Button v-if="loggedIn" asChild variant="outline">
            <RouterLink to="/logOut">Log Out</RouterLink>
          </Button>
        </div>
      </div>
    </div>
  </nav>
</template>
