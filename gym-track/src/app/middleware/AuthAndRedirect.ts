import {
  NavigationGuardReturn,
  RouteLocationNormalizedGeneric,
  Router,
  RouteRecordNormalized,
} from 'vue-router';
import { getCurrentUser } from '../auth/Auth';
import router from '@/Router';
import { currentUser } from '../state/AuthState';

let resolvedAuthAndRedirect = false;

export async function handleAuthAndRedirectBefore(
  to: RouteLocationNormalizedGeneric,
  from: RouteLocationNormalizedGeneric
): Promise<NavigationGuardReturn> {
  if (resolvedAuthAndRedirect) {
    return;
  }
  resolvedAuthAndRedirect = true;

  let evaluatedUser = false;
  if (from.query.redirect) {
    const redirectRoute = getRouteByFullPath(
      router,
      from.query.redirect.toString()
    );

    if (redirectRoute) {
      if (redirectRoute.meta.requiresAuth) {
        currentUser.value = await getCurrentUser();
        evaluatedUser = true;
        if (currentUser.value) {
          return { path: from.query.redirect.toString() };
        }
      } else {
        getCurrentUser().then(user => (currentUser.value = user));
        return { path: from.query.redirect.toString() };
      }
    }
  }

  if (!to.meta.requiresAuth) {
    getCurrentUser().then(user => (currentUser.value = user));
    return;
  }

  if (!evaluatedUser) {
    currentUser.value = await getCurrentUser();
  }

  if (!currentUser.value) {
    return {
      name: 'Log In',
      query: { redirect: to.path },
    };
  }

  if (!currentUser.value.isEmailConfirmed) {
    return {
      name: 'Confirm Email',
      query: { email: currentUser.value.email, redirect: to.path },
    };
  }
}

export function handleAuthAndRedirectAfter(): void {
  resolvedAuthAndRedirect = false;
}

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
