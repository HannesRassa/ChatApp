import axios from 'axios';
import { useUserStore } from '@/stores/userStore';

const axiosInstance = axios.create({
  baseURL: 'https://localhost:7269/Backend/',
});

axiosInstance.interceptors.request.use(
  (config) => {
    const userStore = useUserStore();
    if (userStore.token) {
      config.headers.Authorization = `Bearer ${userStore.token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

axiosInstance.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      const userStore = useUserStore();
      userStore.logOut(); // Log out the user on token expiration
      window.location.href = '/'; // Redirect to login
    }
    return Promise.reject(error);
  }
);
export const isTokenExpired = (tokenExpiration: string): boolean => {
    const expirationTime = new Date(tokenExpiration).getTime();
    return Date.now() > expirationTime;
};
  

export default axiosInstance;
