import axios from 'axios'
import { useAuthStore } from '../features/auth/auth.store'

const api = axios.create({
   baseURL: import.meta.env.VITE_API_URL || import.meta.env.VITE_API_BASE_URL,
    headers: {
      "Content-Type": "application/json",
    },
})

api.interceptors.request.use(config => {
  const token = localStorage.getItem('access_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.request.use(config => {
  const auth = useAuthStore()
  if (auth.accessToken) {
    config.headers.Authorization = `Bearer ${auth.accessToken}`
  }
  return config
})


api.interceptors.response.use(
  res => res,
  async err => {
    const auth = useAuthStore()
    const original = err.config

    if (err.response?.status === 401 && !original._retry) {
      original._retry = true

      const res = await axios.post(
        'http://localhost:5000/api/auth/refresh',
        { refreshToken: auth.refreshToken }
      )

      auth.setTokens(res.data)
      original.headers.Authorization =
        `Bearer ${res.data.accessToken}`

      return api(original)
    }

    return Promise.reject(err)
  }
)

export default api
