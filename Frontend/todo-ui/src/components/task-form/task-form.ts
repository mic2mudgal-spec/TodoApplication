import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup } from '@angular/forms';

import { TASK_STATUS_OPTIONS } from '../../shared/constants/task-status-options';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './task-form.html',
  styleUrl: './task-form.css'
})
export class TaskFormComponent {

  readonly taskForm =
    input.required<FormGroup>();

  readonly isEditMode =
    input(false);

  readonly save =
    output<void>();

  readonly cancel =
    output<void>();

  readonly statusList = TASK_STATUS_OPTIONS;

}
