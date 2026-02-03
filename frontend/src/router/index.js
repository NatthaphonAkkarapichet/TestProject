import { createRouter, createWebHistory } from 'vue-router'
import Login from '../features/auth/pages/Login.vue'
import Dashboard from '../views/Dashboard.vue'
import Register from '../views/Register.vue'

const routes = [
    { path: '/',
    component: Login,
    meta: { layout: 'auth' }
    },
   { path: '/login',
    component: Login,
    meta: { layout: 'auth' }
    },
   {
     path: '/dashboard',
    component: Dashboard,
    meta: { requiresAuth: true },
   },
   {
     path: '/register',
    component: Register,
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('access_token')

  if (to.meta.requiresAuth && !token) {
    next('/login')
  } else {
    next()
  }
})

export default router
