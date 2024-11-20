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
import ResetPasswordFailure from './components/pages/auth/ResetPasswordFailure.vue';
import LogOut from './components/pages/auth/LogOut.vue';
import Workouts from './components/pages/app/Workouts.vue';
import NotFound from './components/pages/NotFound.vue';
import {
  handleAuthAndRedirect as handleAuthAndRedirectBefore,
  handleAuthAndRedirectAfter,
} from './scripts/middleware/AuthAndRedirect';
import Dashboard from './components/pages/app/Dashboard.vue';
import Exercises from './components/pages/app/Exercises.vue';

declare module 'vue-router' {
  enum UserRole {
    User,
    Admin,
  }

  interface IRouteMeta {
    requiresAuth?: boolean;
  }
}

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home,
  },
  {
    path: '/home',
    redirect: { name: 'Home' },
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
    meta: { requiresAuth: true },
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
    path: '/dashboard',
    name: 'Dashboard',
    component: Dashboard,
    meta: { requiresAuth: true },
  },
  {
    path: '/workouts',
    name: 'Workouts',
    component: Workouts,
    meta: { requiresAuth: true },
  },
  {
    path: '/exercises',
    name: 'Exercises',
    component: Exercises,
    meta: { requiresAuth: true },
  },
  {
    path: '/:catchAll(.*)',
    name: 'Not Found',
    component: NotFound,
  },
];

const router = createRouter({
  history: createWebHistory('/'),
  routes,
});

router.beforeEach(handleAuthAndRedirectBefore);
router.afterEach(handleAuthAndRedirectAfter);

export default router;
