import axios from 'axios';

export const apiClient = axios.create({
  baseURL: 'https://localhost:7050/',
  timeout: 10000,
  validateStatus: (status): boolean => true, // never throw
});
