<script setup lang="ts">
import { useAuth } from '@/features/auth/stores/use-auth';
import { Alert, AlertTitle } from '@/features/shared/components/ui/alert';

const auth = useAuth();
auth.logOut();
</script>

<template>
  <Alert>
    <AlertTitle>Logging out...</AlertTitle>
  </Alert>
</template>
