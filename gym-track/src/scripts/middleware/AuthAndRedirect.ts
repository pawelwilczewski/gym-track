import {
  NavigationGuardReturn,
  RouteLocationNormalizedGeneric,
  Router,
  RouteRecordNormalized,
} from 'vue-router';
import { getCurrentUser, IUserInfo } from '../auth/Auth';
import router from '@/Router';

let resolvedAuthAndRedirect = false;

export async function handleAuthAndRedirect(
  to: RouteLocationNormalizedGeneric,
  from: RouteLocationNormalizedGeneric
): Promise<NavigationGuardReturn> {
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
