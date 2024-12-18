<script setup lang="ts">
import { Alert, AlertTitle } from '@/components/ui/alert';
import { logOut } from '@/app/auth/Auth';

logOut();
</script>

<template>
  <Alert>
    <AlertTitle>Logging out...</AlertTitle>
  </Alert>
</template>
