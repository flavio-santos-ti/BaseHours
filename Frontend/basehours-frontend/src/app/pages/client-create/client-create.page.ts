import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ClientService } from '../../services/client.service';

@Component({
  selector: 'app-client-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './client-create.page.html',
})
export class ClientCreatePage {
  errorMessage = '';

  form = this.fb.group({
    name: ['', Validators.required],
  });

  constructor(
    private fb: FormBuilder,
    private clientService: ClientService,
    public router: Router
  ) {}

  onSubmit() {
    this.errorMessage = '';

    if (this.form.valid) {
      this.clientService
        .createClient(this.form.value as { name: string })
        .subscribe(() => {
          this.router.navigate(['/client']);
        });
    }

    if (this.form.valid) {
      this.clientService
        .createClient(this.form.value as { name: string })
        .subscribe({
          next: () => this.router.navigate(['/client']),
          error: (error) => {
            this.errorMessage = error?.error?.message || 'Erro ao criar cliente.';
          },
        });
    
    }

  }

}
