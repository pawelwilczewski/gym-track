import {
  createRouter,
  createWebHistory,
  Router,
  RouteRecordNormalized,
} from 'vue-router';
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
import { getCurrentUser, IUserInfo } from './scripts/auth/Auth';
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

function getRouteByFullPath(
  router: Router,
  path: string
): RouteRecordNormalized | undefined {
  const pathNormalized = path.split('?')[0];
  const routesMatching = router
    .getRoutes()
    .filter(route => route.path === pathNormalized);

  if (routesMatching.length <= 0) {
    console.error(`Invalid route: ${path}`);
    return undefined;
  }

  if (routesMatching.length > 1) {
    // TODO Pawel: router probably checks for duplicates automatically, this may be redundant
    console.error(`Duplicate route (proceeding with first match): ${path}`);
  }

  return routesMatching[0];
}

let resolvedAuthAndRedirect = false;

router.beforeEach(async (to, from) => {
  if (resolvedAuthAndRedirect) {
    return;
  }
  resolvedAuthAndRedirect = true;

  let user: IUserInfo | undefined;
  if (from.query.redirect) {
    const redirectRoute = getRouteByFullPath(
      router,
      from.query.redirect.toString()
    );

    if (redirectRoute) {
      if (redirectRoute.meta.requiresAuth) {
        user = await getCurrentUser();
        if (user) {
          return { path: from.query.redirect.toString() };
        }
      } else {
        return { path: from.query.redirect.toString() };
      }
    }
  }

  if (!to.meta.requiresAuth) {
    return;
  }

  user ??= await getCurrentUser();

  if (!user) {
    return {
      name: 'Log In',
      query: { redirect: to.path },
    };
  }

  if (!user.isEmailConfirmed) {
    return { name: 'Confirm Email' };
  } else if (to.name === 'Confirm Email') {
    return { name: 'Home' };
  }
});

router.afterEach(() => {
  resolvedAuthAndRedirect = false;
});

export default router;
