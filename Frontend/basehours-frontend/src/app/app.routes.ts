import { Routes } from '@angular/router';
import { ClientPage } from './pages/clients/client.page';

export const routes: Routes = [
  {
    path: '',
    component: ClientPage,
  },
  {
    path: 'client/create',
    loadComponent: () =>
      import('./pages/client-create/client-create.page').then(
        (m) => m.ClientCreatePage
      ),
  }  
];
