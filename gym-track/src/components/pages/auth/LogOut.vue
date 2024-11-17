<script setup lang="ts">
import { Alert, AlertTitle } from '@/components/ui/alert';
import { logOut } from '@/scripts/auth/Auth';

logOut();
</script>

<template>
  <Alert>
    <AlertTitle>Logging out...</AlertTitle>
  </Alert>
</template>
