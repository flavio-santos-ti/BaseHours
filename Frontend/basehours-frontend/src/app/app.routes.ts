import { Routes } from '@angular/router';
import { ClientListPage  } from './pages/client-list/client-list.page';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/client-list/client-list.page').then(
        (m) => m.ClientListPage 
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
      import('./pages/client-list/client-list.page').then(
        (m) => m.ClientListPage
      ),
  },
  {
    path: 'clients/edit/:id',
    loadComponent: () =>
      import('./pages/client-edit/client-edit.page').then(m => m.ClientEditPage)
  } 

];
