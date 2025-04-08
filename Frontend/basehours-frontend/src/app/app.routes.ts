import { Routes } from '@angular/router';
import { ClientPage } from './pages/clients/client-list.page';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/clients/client-list.page').then(
        (m) => m.ClientPage
      ),
  },  
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
      import('./pages/clients/client-list.page').then(
        (m) => m.ClientPage
      ),
  }  

];
