import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';



import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { TaskService } from './services/task.service';

import { Task } from './models/task';
import { TaskStatus } from './models/task-status';

import { CreateTaskRequest } from './models/create-task-request';
import { UpdateTaskRequest } from './models/update-task-request';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {

  private readonly fb = inject(FormBuilder);
 private readonly cdr = inject(ChangeDetectorRef);
  private readonly taskService = inject(TaskService);

  tasks: Task[] = [];

  isEditMode = false;

  selectedTaskId = '';

  successMessage = '';

  errorMessage = '';

  readonly statusList = [
    {
      value: TaskStatus.NotStarted,
      text: 'Not Started'
    },
    {
      value: TaskStatus.InProgress,
      text: 'In Progress'
    },
    {
      value: TaskStatus.Completed,
      text: 'Completed'
    }
  ];

  taskForm = this.fb.group({

    name: [
      '',
      [
        Validators.required,
        Validators.maxLength(100)
      ]
    ],

    priority: [
      1,
      [
        Validators.required,
        Validators.min(1)
      ]
    ],

    status: [
      TaskStatus.NotStarted,
      Validators.required
    ]

  });

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {

  this.taskService.getTasks().subscribe({

   next: (response) => {

    this.tasks = response;

    this.cdr.detectChanges();

},

    error: (error) => {

      console.error(error);

      this.errorMessage = this.getErrorMessage(error);

    }

  });

}

  save(): void {

    this.successMessage = '';

    this.errorMessage = '';

    if (this.taskForm.invalid) {

      this.taskForm.markAllAsTouched();

      return;

    }

    if (this.isEditMode) {

      this.updateTask();

    }
    else {

      this.createTask();

    }

  }

  private createTask(): void {

    const request: CreateTaskRequest = {

      name: this.taskForm.value.name!,

      priority: Number(
        this.taskForm.value.priority
      ),

      status: Number(
        this.taskForm.value.status
      ) as TaskStatus

    };

    this.taskService
      .createTask(request)
      .subscribe({

        next: () => {

          this.successMessage =
            'Task created successfully.';

          this.clear();

          this.loadTasks();

        },

        error: (error)  => {

          this.errorMessage = this.getErrorMessage(error);


        }

      });

  }

  private updateTask(): void {

    const request: UpdateTaskRequest = {

      id: this.selectedTaskId,

      name: this.taskForm.value.name!,

      priority: Number(
        this.taskForm.value.priority
      ),

      status: Number(
        this.taskForm.value.status
      ) as TaskStatus

    };

    this.taskService
      .updateTask(request)
      .subscribe({

        next: () => {

          this.successMessage =
            'Task updated successfully.';

          this.clear();

          this.loadTasks();

        },

        error: (error)  => {

        this.errorMessage = this.getErrorMessage(error);


        }

      });

  }

  edit(task: Task): void {

    this.isEditMode = true;

    this.selectedTaskId = task.id;

    this.taskForm.patchValue({

      name: task.name,

      priority: task.priority,

      status: task.status

    });

  }

  delete(task: Task): void {

    if (!confirm(
      `Delete "${task.name}" ?`
    )) {

      return;

    }

    this.taskService
      .deleteTask(task.id)
      .subscribe({

        next: () => {

          this.successMessage =
            'Task deleted successfully.';

          this.loadTasks();

        },

       error: (error)  => {

          this.errorMessage = this.getErrorMessage(error);


        }

      });

  }

  clear(): void {

    this.isEditMode = false;

    this.selectedTaskId = '';

    this.taskForm.reset({

      name: '',

      priority: 1,

      status: TaskStatus.NotStarted

    });

  }

  getStatus(
    status: TaskStatus
  ): string {

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

  private getErrorMessage(error: any): string {

  if (!error) {
    return 'An unexpected error occurred.';
  }

  // Backend returns { message: "..." }
  if (error.error?.message) {
    return error.error.message;
  }

  // Backend returns { Message: "..." }
  if (error.error?.Message) {
    return error.error.Message;
  }

  // Backend returns plain text
  if (typeof error.error === 'string') {
    return error.error;
  }

  // HttpErrorResponse message
  if (error.message) {
    return error.message;
  }

  return 'An unexpected error occurred.';
}

}