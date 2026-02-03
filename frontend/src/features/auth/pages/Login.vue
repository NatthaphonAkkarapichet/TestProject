<script setup>
import { ref, computed } from "vue";
import { useAuthStore } from "../auth.store";
import { useRouter } from "vue-router";

const username = ref("");
const password = ref("");
const loading = ref(false);
const submitted = ref(false);
const auth = useAuthStore();
const router = useRouter();

const showPassword = ref(false);

// ===== validation =====
const usernameError = computed(() =>
  !username.value ? "กรุณากรอก Username" : "",
);

const passwordError = computed(() => {
  if (!password.value) return "กรุณากรอก Password";
  if (password.value.length < 6) return "Password ต้องอย่างน้อย 6 ตัว";
  return "";
});

const isValid = computed(() => !usernameError.value && !passwordError.value);

const submit = async () => {
  submitted.value = true;
  if (!isValid.value) return;

  loading.value = true;
  try {
    await auth.login({
      username: username.value,
      password: password.value,
    });
    alert("Login success");
    console.log("token:", localStorage.getItem("access_token")); // ต้องไม่ null
    router.push("/dashboard");
  } catch (err) {
    const message =
      err.response?.data?.message || // backend ส่ง message มา
      err.response?.data || // fallback
      "Login failed";

    alert(message);
  } finally {
    loading.value = false;
  }
};
</script>

<template>
  <div class="login-wrapper">
    <Card class="login-card">
      <template #title>
        <div class="text-center text-xl font-semibold mb-6">Login</div>
      </template>

      <template #content>
        <!-- Username -->
        <div class="grid align-items-center mb-4">
          <label class="col-12 md:col-4 font-medium"> Username </label>
          <div class="col-12 md:col-8">
            <InputText
              v-model="username"
              class="w-full"
              placeholder="Enter username"
              :class="{ 'p-invalid': submitted && usernameError }"
            />
            <small v-if="submitted && usernameError" class="p-error">
              {{ usernameError }}
            </small>
          </div>
        </div>

        <!-- Password -->
        <div class="grid align-items-center mb-4">
          <label class="col-12 md:col-4 font-medium"> Password </label>
          <div class="col-12 md:col-8">
            <Password
              v-model="password"
              :feedback="false"
              input-class="w-full"
              placeholder="Enter password"
              :class="{ 'p-invalid': submitted && passwordError }"
            />
            <small v-if="submitted && passwordError" class="p-error">
              {{ passwordError }}
            </small>
          </div>
        </div>

        <!-- Button -->
        <div class="flex justify-content-center mt-3">
          <Button label="Login" icon="pi pi-sign-in" @click="submit" />
        </div>

        <!-- Register -->
        <div class="text-center mt-3">
          <a class="register-link" @click.prevent="$router.push('/register')">
            สมัครสมาชิก
          </a>
        </div>
      </template>
    </Card>
  </div>
</template>

<style scoped>
.login-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
}

.login-card {
  width: 28rem;
}

.register-link {
  color: #6366f1;
  text-decoration: none;
  font-weight: 500;
}

.register-link:hover {
  text-decoration: underline;
}

.p-error {
  font-size: 0.75rem;
  color: red;
}
</style>
