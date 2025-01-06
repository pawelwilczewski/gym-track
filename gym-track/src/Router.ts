import { createRouter, createWebHistory } from 'vue-router';
import LogIn from './features/auth/views/LogIn.vue';
import Home from './features/shared/views/Home.vue';
import SignUp from './features/auth/views/SignUp.vue';
import ConfirmEmail from './features/auth/views/ConfirmEmail.vue';
import ConfirmedEmail from './features/auth/views/ConfirmedEmail.vue';
import ForgotPassword from './features/auth/views/ForgotPassword.vue';
import ConfirmEmailChange from './features/auth/views/ConfirmEmailChange.vue';
import ResetPasswordSuccess from './features/auth/views/ResetPasswordSuccess.vue';
import Lockout from './features/auth/views/Lockout.vue';
import ResetPasswordFailure from './features/auth/views/ResetPasswordFailure.vue';
import LogOut from './features/auth/views/LogOut.vue';
import Workouts from './features/workout/views/Workouts.vue';
import NotFound from './features/shared/views/NotFound.vue';
import {
  handleAuthAndRedirectBefore,
  handleAuthAndRedirectAfter,
} from './features/auth/middleware/AuthAndRedirect';
import Dashboard from './features/dashboard/views/Dashboard.vue';
import ExerciseInfos from './features/exerciseInfo/views/ExerciseInfos.vue';

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

export default router;
