<script setup>
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import { registerApi } from "../features/auth/auth.service";

const router = useRouter();

const username = ref("");
const password = ref("");
const confirmPassword = ref("");
const submitted = ref(false);
const loading = ref(false);

// ===== validation =====
const usernameError = computed(() =>
  !username.value ? "กรุณากรอก Username" : "",
);

const passwordError = computed(() => {
  if (!password.value) return "กรุณากรอก Password";
  if (password.value.length < 6) return "Password ต้องอย่างน้อย 6 ตัว";
  return "";
});

const confirmPasswordError = computed(() => {
  if (!confirmPassword.value) return "กรุณายืนยัน Password";
  if (confirmPassword.value !== password.value) return "Password ไม่ตรงกัน";
  return "";
});

const isValid = computed(
  () =>
    !usernameError.value && !passwordError.value && !confirmPasswordError.value,
);

const submit = async () => {
  submitted.value = true;
  if (!isValid.value) return;

  loading.value = true;
  try {
    await registerApi({
      username: username.value,
      password: password.value,
      passwordConfirm: confirmPassword.value,
    });

    alert("สมัครสมาชิกสำเร็จ");
    router.push("/login");
  } catch (err) {
    const message =
      err.response?.data?.message || // backend ส่ง message มา
      err.response?.data || // fallback
      "สมัครสมาชิกไม่สำเร็จ";

    alert(message);
  } finally {
    loading.value = false;
  }
};
</script>

<template>
  <div class="register-wrapper">
    <Card class="register-card">
      <template #title>
        <div class="text-center">Register</div>
      </template>

      <template #content>
        <!-- Username -->
        <div class="field">
          <label>Username</label>
          <InputText
            v-model="username"
            class="w-full"
            :class="{ 'p-invalid': submitted && usernameError }"
          />
          <small v-if="submitted && usernameError" class="p-error">
            {{ usernameError }}
          </small>
        </div>

        <!-- Password -->
        <div class="field">
          <label>Password</label>
          <InputText
            v-model="password"
            type="password"
            class="w-full"
            :class="{ 'p-invalid': submitted && passwordError }"
          />
          <small v-if="submitted && passwordError" class="p-error">
            {{ passwordError }}
          </small>
        </div>

        <!-- Confirm -->
        <div class="field">
          <label>Confirm Password</label>
          <InputText
            v-model="confirmPassword"
            type="password"
            class="w-full"
            :class="{ 'p-invalid': submitted && confirmPasswordError }"
          />
          <small v-if="submitted && confirmPasswordError" class="p-error">
            {{ confirmPasswordError }}
          </small>
        </div>

        <Button
          label="สมัครสมาชิก"
          icon="pi pi-user-plus"
          class="w-full mt-3"
          :loading="loading"
          @click="submit"
        />
      </template>
    </Card>
  </div>
</template>

<style scoped>
.register-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
}

.register-card {
  width: 26rem;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-bottom: 1rem;
}
.p-error {
  font-size: 0.75rem;
  color: red;
}
</style>
