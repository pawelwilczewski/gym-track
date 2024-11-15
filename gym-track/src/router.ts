import { createRouter, createWebHistory } from 'vue-router';
import LogIn from './pages/auth/LogIn.vue';
import Home from './pages/Home.vue';
import SignUp from './pages/auth/SignUp.vue';
import ConfirmEmail from './pages/auth/ConfirmEmail.vue';
import ConfirmedEmail from './pages/auth/ConfirmedEmail.vue';
import ForgotPassword from './pages/auth/ForgotPassword.vue';
import ConfirmEmailChange from './pages/auth/ConfirmEmailChange.vue';
import ResetPasswordSuccess from './pages/auth/ResetPasswordSuccess.vue';
import Lockout from './pages/auth/Lockout.vue';
import SignUpConfirmation from './pages/auth/SignUpConfirmation.vue';
import ResetPasswordFailure from './pages/auth/ResetPasswordFailure.vue';

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
    path: '/confirmEmailChange',
    name: 'Confirm Email Change',
    component: ConfirmEmailChange,
  },
  {
    path: '/forgotPassword',
    name: 'Forgot Password',
    component: ForgotPassword,
  },
  {
    path: '/resetPasswordSuccess',
    name: 'Reset Password Success',
    component: ResetPasswordSuccess,
  },
  {
    path: '/resetPasswordFailure',
    name: 'Reset Password Failure',
    component: ResetPasswordFailure,
  },
  {
    path: '/lockout',
    name: 'Lockout',
    component: Lockout,
  },
  {
    path: '/signUpConfirmation',
    name: 'Sign Up Confirmation',
    component: SignUpConfirmation,
  },
];

const router = createRouter({
  history: createWebHistory('/'),
  routes,
});

export default router;
