import { Routes } from '@angular/router';
import { ClientPage } from './pages/clients/client.page';

export const routes: Routes = [
  {
    path: 'client/create',
    loadComponent: () =>
      import('./pages/client-create/client-create.page').then(
        (m) => m.ClientCreatePage
      ),
  },
  {
    path: 'client',
    loadComponent: () =>
      import('./pages/clients/client.page').then(
        (m) => m.ClientPage
      ),
  }  

];
