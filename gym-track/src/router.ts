import { createRouter, createWebHistory } from 'vue-router';
import LogIn from './pages/auth/LogIn.vue';
import Home from './pages/Home.vue';
import SignUp from './pages/auth/SignUp.vue';
import ConfirmEmail from './pages/auth/ConfirmEmail.vue';
import ConfirmedEmail from './pages/auth/ConfirmedEmail.vue';
import ForgotPassword from './pages/auth/ForgotPassword.vue';

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
  {
    path: '/confirmEmail',
    name: 'Confirm Email',
    component: ConfirmEmail,
  },
  {
    path: '/confirmedEmail',
    name: 'Confirmed Email',
    component: ConfirmedEmail,
  },
  {
    path: '/forgotPassword',
    name: 'Forgot Password',
    component: ForgotPassword,
  },
];

const router = createRouter({
  history: createWebHistory('/'),
  routes,
});

export default router;
