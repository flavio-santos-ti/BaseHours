import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ClientService } from '../../services/client.service';

@Component({
  selector: 'app-client-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './client-edit.page.html',
  styleUrls: ['./client-edit.page.scss']
})
export class ClientEditPage implements OnInit {
  form: FormGroup;
  isLoading = false;
  errorMessage: string = '';
  clientId: string = '';

  constructor(
    private fb: FormBuilder,
    private clientService: ClientService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.clientId = this.route.snapshot.paramMap.get('id') || '';
    if (this.clientId) {
      this.loadClient(this.clientId);
    }
  }

  loadClient(id: string) {
    this.isLoading = true;
    this.clientService.getById(id).subscribe({
      next: (response) => {
        this.form.patchValue({ name: response.data.name });
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Erro ao carregar cliente.';
        this.isLoading = false;
      }
    });
  }

  onSubmit() {
    if (this.form.invalid || this.isLoading) return;
  
    this.isLoading = true;
  
    const payload = {
      id: this.clientId,
      name: this.form.value.name,
    };
  
    this.clientService.update(payload).subscribe({
      next: () => {
        this.router.navigate(['/client']);
      },
      error: () => {
        this.errorMessage = 'Erro ao atualizar cliente.';
        this.isLoading = false;
      },
    });
  }
  
  goBack() {
    this.router.navigate(['/client']);
  }
  
}
