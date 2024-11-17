import { createRouter, createWebHistory } from 'vue-router';
import LogIn from './components/pages/auth/LogIn.vue';
import Home from './components/pages/Home.vue';
import SignUp from './components/pages/auth/SignUp.vue';
import ConfirmEmail from './components/pages/auth/ConfirmEmail.vue';
import ConfirmedEmail from './components/pages/auth/ConfirmedEmail.vue';
import ForgotPassword from './components/pages/auth/ForgotPassword.vue';
import ConfirmEmailChange from './components/pages/auth/ConfirmEmailChange.vue';
import ResetPasswordSuccess from './components/pages/auth/ResetPasswordSuccess.vue';
import Lockout from './components/pages/auth/Lockout.vue';
import SignUpConfirmation from './components/pages/auth/SignUpConfirmation.vue';
import ResetPasswordFailure from './components/pages/auth/ResetPasswordFailure.vue';
import LogOut from './components/pages/auth/LogOut.vue';

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
    path: '/logOut',
    name: 'Log Out',
    component: LogOut,
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
