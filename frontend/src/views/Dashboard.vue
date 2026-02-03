<script setup>
import { useAuthStore } from "../features/auth/auth.store";
const auth = useAuthStore();
</script>

<template>
  <div class="center">
    <h2>Welcome, {{ auth.username }}</h2>
  </div>
</template>

<style scoped>
.center {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
}
</style>
