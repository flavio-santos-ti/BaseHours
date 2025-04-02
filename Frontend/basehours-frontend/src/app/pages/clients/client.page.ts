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

  deleteSelectedClient(): void {
    if (this.selectedClientId) {
      this.clients = this.clients.filter(c => c.id !== this.selectedClientId);
      this.selectedClientId = null;
    }
  }  
}
