import { createRouter, createWebHistory } from 'vue-router';
import LogIn from './features/auth/views/login.vue';
import Home from './features/shared/views/home.vue';
import SignUp from './features/auth/views/signup.vue';
import ConfirmEmail from './features/auth/views/confirm-email.vue';
import ConfirmedEmail from './features/auth/views/confirmed-email.vue';
import ForgotPassword from './features/auth/views/forgot-password.vue';
import ConfirmEmailChange from './features/auth/views/confirm-email-change.vue';
import ResetPasswordSuccess from './features/auth/views/reset-password-success.vue';
import Lockout from './features/auth/views/lockout.vue';
import ResetPasswordFailure from './features/auth/views/reset-password-failure.vue';
import LogOut from './features/auth/views/logout.vue';
import Workouts from './features/workout/views/workouts.vue';
import NotFound from './features/shared/views/not-found.vue';
import {
  handleAuthAndRedirectBefore,
  handleAuthAndRedirectAfter,
} from './features/auth/middleware/auth-and-redirect';
import Dashboard from './features/dashboard/views/dashboard.vue';
import ExerciseInfos from './features/exercise-info/views/exercise-infos.vue';
import { fetchAntiforgeryToken } from '@/features/antiforgery/middleware/fetch-antiforgery-token';

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
    path: '/login',
    name: 'Log In',
    component: LogIn,
  },
  {
    path: '/logout',
    name: 'Log Out',
    component: LogOut,
  },
  {
    path: '/signup',
    name: 'Sign Up',
    component: SignUp,
  },
  {
    path: '/confirm-email',
    name: 'Confirm Email',
    component: ConfirmEmail,
  },
  {
    path: '/confirmed-email',
    name: 'Confirmed Email',
    component: ConfirmedEmail,
  },
  {
    path: '/confirm-email-change',
    name: 'Confirm Email Change',
    component: ConfirmEmailChange,
    meta: { requiresAuth: true },
  },
  {
    path: '/forgot-password',
    name: 'Forgot Password',
    component: ForgotPassword,
  },
  {
    path: '/reset-password-success',
    name: 'Reset Password Success',
    component: ResetPasswordSuccess,
  },
  {
    path: '/reset-password-failure',
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
    name: 'Exercise Infos',
    component: ExerciseInfos,
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
router.afterEach(fetchAntiforgeryToken);

export default router;
