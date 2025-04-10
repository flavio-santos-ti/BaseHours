import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Router } from '@angular/router';
import { ClientService } from '../../services/client.service';
import { ConfirmDialogComponent } from '../../components/confirm-dialog/confirm-dialog.component';


@Component({
  selector: 'app-client',                      
  standalone: true,
  imports: [
    CommonModule,
    ConfirmDialogComponent,
    RouterModule 
  ],
  templateUrl: './client-list.page.html',           
  styleUrls: ['./client-list.page.scss'],           
})
export class ClientListPage implements OnInit {    

  clients: any[] = [];
  selectedClientId: string | null = null;
  isLoading = false;
  isNavigating = false;
  isDeleting = false;
  isAdding = false;
  isEditing = false;
  showConfirmDialog = false;


  constructor(private clientService: ClientService, private router: Router) {}

  ngOnInit(): void {
    this.loadClients();
  }


  loadClients() {
    this.isLoading = true;
  
    this.clientService.getClients().subscribe({
      next: (clients) => {
        this.clients = clients.data;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar clientes:', error);
        this.isLoading = false;
      }
    });
  }
  
  selectClient(client: any): void {
    this.selectedClientId = client.id;
  }  

  editSelectedClient() {
    if (!this.selectedClientId || this.isEditing) return;
  
    this.isEditing = true;

    this.router.navigate(['/clients/edit', this.selectedClientId]);
  }
  
  // Método para excluir o cliente selecionado
  deleteSelectedClient(): void {

    if (this.selectedClientId) {
      this.clientService.deleteClient(this.selectedClientId).subscribe({
        next: () => {
          // Atualize a lista de clientes removendo o cliente excluído
          this.clients = this.clients.filter(client => client.id !== this.selectedClientId);
          this.selectedClientId = null; // Limpa a seleção
          alert('Client deleted successfully');
        },
        error: (err) => {
          console.error('Error deleting client:', err);
          alert('Failed to delete the client');
        }
      });
    }
  }  

  confirmDelete() {
    if (!this.selectedClientId || this.isDeleting) return;
  
    this.showConfirmDialog = true;
  }
  
  onDialogConfirm() {
    if (!this.selectedClientId) return;
  
    this.isDeleting = true;
  
    this.clientService.deleteClient(this.selectedClientId!).subscribe({
      next: () => {
        this.clients = this.clients.filter(c => c.id !== this.selectedClientId);
        this.selectedClientId = null;
        this.showConfirmDialog = false;
        this.isDeleting = false;
      },
      error: () => {
        this.showConfirmDialog = false;
        this.isDeleting = false;
        // Se tiver tratamento de erro, pode setar errorMessage aqui
      },
    });
  }
      
  onDialogCancel() {
    this.showConfirmDialog = false;
  }

  goToCreatePage() {
    if (this.isAdding ) return;
  
    this.isAdding  = true;

    this.router.navigate(['/client/create']).finally(() => {
      this.isAdding  = false;
    });
  }
 
}
