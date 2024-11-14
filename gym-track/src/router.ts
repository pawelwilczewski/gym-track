import { createRouter, createWebHistory } from 'vue-router';
import LogIn from './pages/auth/LogIn.vue';
import Home from './pages/Home.vue';
import SignUp from './pages/auth/SignUp.vue';

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home,
  },
  {
    path: '/logIn',
    name: 'Log In',
    component: LogIn,
  },
  {
    path: '/signUp',
    name: 'Sign Up',
    component: SignUp,
  },
];

const router = createRouter({
  history: createWebHistory('/'),
  routes,
});

export default router;
