import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ClientService } from '../../services/client.service';
import { ConfirmDialogComponent } from '../../components/confirm-dialog.component';


@Component({
  selector: 'app-client',                      
  standalone: true,
  imports: [
    CommonModule,
    ConfirmDialogComponent,
    RouterModule 
  ],
  templateUrl: './client.page.html',           
  styleUrls: ['./client.page.scss'],           
})
export class ClientPage implements OnInit {    

  clients: any[] = [];
  selectedClientId: string | null = null;
  isLoading = false;

  constructor(private clientService: ClientService) {}

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

  editSelectedClient(): void {
    if (this.selectedClientId) {
      // Exemplo: navegação para tela de edição
      console.log('Editar cliente com ID:', this.selectedClientId);
    }
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
  
  showConfirmDialog = false;

  confirmDelete() {
    this.showConfirmDialog = true;
  }
  
  onDialogConfirm() {
    this.deleteSelectedClient();
    this.showConfirmDialog = false;
  }
  
  onDialogCancel() {
    this.showConfirmDialog = false;
  }
 
}
