<script setup lang="ts">
import { Button } from '@/features/shared/components/ui/button';
import TabButton from '@/features/shared/components/tab-button.vue';
import { useAuth } from '@/features/auth/stores/use-auth';
import { Dumbbell } from 'lucide-vue-next';

const auth = useAuth();
</script>

<template>
  <nav class="flex justify-between gap-24">
    <h1 class="pb-3 px-3">
      <RouterLink to="/" class="font-bold">
        <Dumbbell class="w-12 h-12" />
      </RouterLink>
    </h1>
    <div class="flex gap-6 flex-wrap">
      <div v-if="auth.isLoggedIn" class="flex gap-2">
        <!-- <TabButton path="/dashboard" name="Dashboard"></TabButton> -->
        <TabButton path="/workouts" name="Workouts"></TabButton>
        <TabButton path="/exercises" name="Exercises"></TabButton>
      </div>
      <div class="flex gap-2">
        <Button as-child>
          <RouterLink v-if="!auth.isLoggedIn" to="/login">Log In</RouterLink>
        </Button>
        <Button v-if="!auth.isLoggedIn" as-child variant="outline">
          <RouterLink to="/signup">Sign Up</RouterLink>
        </Button>
        <Button v-if="auth.isLoggedIn" as-child variant="outline">
          <RouterLink to="/logout">Log Out</RouterLink>
        </Button>
      </div>
    </div>
  </nav>
</template>
