import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Task } from '../../app/models/task';
import { TaskStatus } from '../../app/models/task-status';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './task-list.html',
  styleUrl: './task-list.css'
})
export class TaskListComponent {

  readonly tasks =
    input.required<Task[]>();

  readonly isEditMode =
    input(false);

  readonly edit =
    output<Task>();

  readonly delete =
    output<Task>();

  getStatus(status: TaskStatus): string {

    switch (status) {

      case TaskStatus.NotStarted:
        return 'Not Started';

      case TaskStatus.InProgress:
        return 'In Progress';

      case TaskStatus.Completed:
        return 'Completed';

      default:
        return '';
    }

  }

}
