import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ConfirmDialogComponent } from '../../components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-project-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ConfirmDialogComponent],
  templateUrl: './project-list.page.html',
})
export class ProjectListPage {
  projects: { id: string; name: string }[] = [];
  isLoading = false;
  isAdding = false;
  isEditing = false;
  isDeleting = false;
  selectedProjectId: string | null = null;
  showConfirmDialog = false;

  selectProject(project: any) {
    this.selectedProjectId = project.id;
  }

  editSelectedProject() {
    this.isEditing = true;
    // lógica para editar
  }

  confirmDelete() {
    this.showConfirmDialog = true;
  }

  onDialogConfirm() {
    this.showConfirmDialog = false;
    // lógica para deletar
  }

  onDialogCancel() {
    this.showConfirmDialog = false;
  }

  goToCreatePage() {
    // navegação para criar projeto
  }
}
