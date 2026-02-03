import { defineStore } from 'pinia'
import { loginApi } from './auth.service'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    accessToken: localStorage.getItem('access_token'),
    refreshToken: localStorage.getItem('refresh_token'),
    username: localStorage.getItem('username'),
  }),

  actions: {
    async login(payload) {
      const res = await loginApi(payload)

      this.accessToken = res.data.accessToken
      this.refreshToken = res.data.refreshToken
      this.username = res.data.username
      localStorage.setItem('access_token', this.accessToken)
      localStorage.setItem('refresh_token', this.refreshToken)
      localStorage.setItem('username', this.username)


       return res
    },

    logout() {
      this.user = null
      this.token = null
      localStorage.removeItem('access_token')
      localStorage.removeItem('refresh_token')
      localStorage.removeItem('username')
    },

    setTokens(data) {
      this.accessToken = data.accessToken
      this.refreshToken = data.refreshToken
      this.username = data.username
        localStorage.setItem('access_token', data.accessToken)
        localStorage.setItem('refresh_token', data.refreshToken)
        localStorage.setItem('username', data.username)
    },
  }
})
