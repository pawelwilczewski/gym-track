<script setup lang="ts">
import { Button } from '@/components/ui/button';
import { isLoggedIn } from '@/scripts/auth/Auth';
import TabButton from './TabButton.vue';

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
          <TabButton path="/tracking" name="My Progress"></TabButton>
          <TabButton path="/workouts" name="Workouts"></TabButton>
          <TabButton path="/exercises" name="Exercises"></TabButton>
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
