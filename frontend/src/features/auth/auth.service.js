import api from '../../services/api'

export const loginApi = (data) =>
  api.post('auth/login', data)


export const registerApi = (data) =>    
    api.post('auth/register', data)
