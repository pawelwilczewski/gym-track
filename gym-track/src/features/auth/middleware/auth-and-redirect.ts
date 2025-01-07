import {
  NavigationGuardReturn,
  RouteLocationNormalizedGeneric,
  Router,
  RouteRecordNormalized,
} from 'vue-router';
import router from '@/router';
import { useAuth } from '../stores/use-auth';

let resolvedAuthAndRedirect = false;

export async function handleAuthAndRedirectBefore(
  to: RouteLocationNormalizedGeneric,
  from: RouteLocationNormalizedGeneric
): Promise<NavigationGuardReturn> {
  if (resolvedAuthAndRedirect) {
    return;
  }
  resolvedAuthAndRedirect = true;

  const auth = useAuth();

  let evaluatedUser = false;
  if (from.query.redirect) {
    const redirectRoute = getRouteByFullPath(
      router,
      from.query.redirect.toString()
    );

    if (redirectRoute) {
      if (redirectRoute.meta.requiresAuth) {
        await auth.fetchCurrentUser();
        evaluatedUser = true;
        if (auth.currentUser) {
          return { path: from.query.redirect.toString() };
        }
      } else {
        auth.fetchCurrentUser();
        return { path: from.query.redirect.toString() };
      }
    }
  }

  if (!to.meta.requiresAuth) {
    auth.fetchCurrentUser();
    return;
  }

  if (!evaluatedUser) {
    await auth.fetchCurrentUser();
  }

  if (!auth.currentUser) {
    return {
      name: 'Log In',
      query: { redirect: to.path },
    };
  }

  if (!auth.currentUser.isEmailConfirmed) {
    return {
      name: 'Confirm Email',
      query: { email: auth.currentUser.email, redirect: to.path },
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
