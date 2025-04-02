import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClientService } from '../../services/client.service';

@Component({
  selector: 'app-client',                      
  standalone: true,
  imports: [CommonModule],
  templateUrl: './client.page.html',           
  styleUrls: ['./client.page.scss'],           
})
export class ClientPage implements OnInit {    

  clients: any[] = [];

  constructor(private clientService: ClientService) {}

  ngOnInit(): void {
    this.clientService.getClients().subscribe(response => {
      if (response.isSuccess) {
        this.clients = response.data;
      }
    });
  }

  selectedClientId: string | null = null;

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
   
}
