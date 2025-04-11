import { Routes } from '@angular/router';
import { ClientListPage } from './pages/client-list/client-list.page';
import { ClientCreatePage } from './pages/client-create/client-create.page';
import { ClientEditPage } from './pages/client-edit/client-edit.page';
import { HomePageComponent } from './pages/home/home.page'; // adicionar esse import

export const routes: Routes = [
  { path: '', component: HomePageComponent }, // agora a tela inicial está limpa
  { path: 'clients', component: ClientListPage },
  { path: 'clients/create', component: ClientCreatePage },
  { path: 'clients/edit/:id', component: ClientEditPage },
];
