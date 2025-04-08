import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse  } from '../models/api-response.model';
import { Client } from '../models/client.model';


@Injectable({
  providedIn: 'root'
})
export class ClientService {
  private apiUrl = 'http://localhost:5228/api/clients';

  constructor(private http: HttpClient) {}

  getClients(): Observable<ApiResponse<Client[]>> {
    return this.http.get<ApiResponse<Client[]>>(this.apiUrl);
  }
  
  deleteClient(clientId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${clientId}`);
  } 
  
  createClient(data: { name: string }) {
    return this.http.post(`${this.apiUrl}`, data);
  }
    
  getById(id: string): Observable<ApiResponse<Client>> {
    return this.http.get<ApiResponse<Client>>(`${this.apiUrl}/${id}`);
  }

  update(data: { id: string; name: string }): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}`, data); 
  }
    
}
