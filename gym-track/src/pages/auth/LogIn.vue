<script setup lang="ts">
import DefaultLayout from '@/components/layouts/DefaultLayout.vue';
import Button from '@/components/ui/button/Button.vue';
import Input from '@/components/ui/input/Input.vue';
import Label from '@/components/ui/label/Label.vue';
</script>

<template>
  <DefaultLayout>
    <div class="mx-8">
      <form
        class="mx-auto border border-border rounded-xl max-w-sm flex flex-col gap-4 p-4"
      >
        <div>
          <Label for="username">Username</Label>
          <Input id="username" type="email" />
        </div>
        <div>
          <Label for="password">Password</Label>
          <Input id="password" type="password" />
        </div>
        <div class="flex justify-between flex-wrap gap-2">
          <a href="/forgotPassword" class="text-sm">Forgot Password?</a>
          <a href="/signUp" class="text-sm">Sign Up</a>
        </div>
        <Button class="mx-auto px-8" type="submit">Log In</Button>
      </form>
    </div>
  </DefaultLayout>
</template>
