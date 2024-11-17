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
import { getCurrentUser } from './scripts/auth/Auth';
import Workouts from './components/pages/app/Workouts.vue';
import NotFound from './components/pages/NotFound.vue';

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
    meta: { requiresAuth: true },
  },
  {
    path: '/confirmedEmail',
    name: 'Confirmed Email',
    component: ConfirmedEmail,
    meta: { requiresAuth: true },
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
    path: '/signUpConfirmation',
    name: 'Sign Up Confirmation',
    component: SignUpConfirmation,
    meta: { requiresAuth: true }, // TODO Pawel: make sure this should be authenticated
  },
  {
    path: '/workouts',
    name: 'Workouts',
    component: Workouts,
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

router.beforeEach(async to => {
  if (!to.meta.requiresAuth) {
    return;
  }

  const user = await getCurrentUser();

  if (!user) {
    return {
      name: 'Log In',
      query: { redirect: to.fullPath },
    };
  }

  if (!user.isEmailConfirmed) {
    return {
      name: 'Confirm Email',
    };
  } else if (to.name === 'Confirm Email') {
    return {
      name: 'Home',
    };
  }
});

export default router;
