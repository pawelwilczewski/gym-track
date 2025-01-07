import axios from 'axios';

export const apiClient = axios.create({
  baseURL: 'https://localhost:7050/',
  timeout: 10_000,
  validateStatus: () => true, // never throw
  withCredentials: true,
});
