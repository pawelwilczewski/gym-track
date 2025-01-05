<script setup lang="ts">
import { useAuth } from '@/app/stores/UseAuth';
import { Alert, AlertTitle } from '@/components/ui/alert';

const auth = useAuth();
auth.logOut();
</script>

<template>
  <Alert>
    <AlertTitle>Logging out...</AlertTitle>
  </Alert>
</template>
