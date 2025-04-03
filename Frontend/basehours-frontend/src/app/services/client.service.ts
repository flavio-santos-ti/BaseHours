import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

interface Client {
  id: string;
  name: string;
}

interface ApiResponse {
  isSuccess: boolean;
  message: string;
  statusCode: number;
  data: Client[];
}

@Injectable({
  providedIn: 'root'
})
export class ClientService {
  private apiUrl = 'http://localhost:5228/api/clients';

  constructor(private http: HttpClient) {}

  getClients(): Observable<ApiResponse> {
    return this.http.get<ApiResponse>(this.apiUrl);
  }

  deleteClient(clientId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${clientId}`);
  } 
  
  createClient(data: { name: string }) {
    return this.http.post(`${this.apiUrl}`, data);
  }
    
}
