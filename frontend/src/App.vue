<script setup>
import { computed } from "vue";
import { useRoute } from "vue-router";

import AuthLayout from "./layouts/AuthLayout.vue";
import DefaultLayout from "./layouts/DefaultLayout.vue";

const route = useRoute();

const layout = computed(() => {
  console.log("layout:", route.meta.layout);
  return route.meta.layout === "auth" ? AuthLayout : DefaultLayout;
});
</script>

<template>
  <component :is="layout">
    <router-view />
  </component>
</template>
