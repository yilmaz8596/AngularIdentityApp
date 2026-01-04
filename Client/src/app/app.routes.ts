import { Routes } from '@angular/router';
import { Home } from './home/home';

export const routes: Routes = [
  {
    path: '',
    component: Home,
  },
  {
    path: 'account',
    loadChildren: () => import('./account/routes').then((m) => m.accountRoutes),
  },
  {
    path: 'not-found',
    loadComponent: () =>
      import('./shared/components/not-found/not-found').then((m) => m.NotFound),
  },
  {
    path: '**',
    redirectTo: 'not-found',
  },
];
