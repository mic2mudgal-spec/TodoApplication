import {
  Component,
  OnInit,
  inject
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';


import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { TaskService } from '../services/task.service';

import { Task } from '../models/task';

import { TaskStatus } from '../models/task-status';

import { CreateTaskRequest } from '../models/create-task-request';

import { UpdateTaskRequest } from '../models/update-task-request';

import { MessageComponent } from '../../components/message/message';

import { TaskFormComponent } from '../../components/task-form/task-form';

import { TaskListComponent } from '../../components/task-list/task-list';

import { getErrorMessage } from '../../shared/utils/http-error.util';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MessageComponent,
    TaskFormComponent,
    TaskListComponent
  ],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class HomeComponent implements OnInit {

  private readonly cdr = inject(ChangeDetectorRef);

  private readonly fb = inject(FormBuilder);

  private readonly taskService = inject(TaskService);

  tasks: Task[] = [];

  successMessage = '';

  errorMessage = '';

  isEditMode = false;

  selectedTaskId = '';

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
        Validators.min(1),
        Validators.max(10)
      ]
    ],

    status: [
      TaskStatus.NotStarted,
      Validators.required
    ]

  });
ngOnInit(): void {

    console.log('Home Loaded');

    this.loadTasks();
this.cdr.detectChanges();
}

  loadTasks(): void {

     console.log('Loading tasks...');
 
    this.taskService
      .getTasks()
      .subscribe({

        next: response => {

          this.tasks = response;
          this.cdr.detectChanges();
        },

        error: error => {

          this.errorMessage =
            getErrorMessage(error);
          this.cdr.detectChanges();
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

      return;

    }
    this.createTask();

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

        error: error => {

          this.errorMessage =
            getErrorMessage(error);
        this.cdr.detectChanges();
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

        error: error => {

          this.errorMessage =
            getErrorMessage(error);
            this.cdr.detectChanges();

        }

      });

  }

  edit(task: Task): void {

    this.successMessage = '';

    this.errorMessage = '';

    this.isEditMode = true;

    this.selectedTaskId = task.id;

    this.taskForm.patchValue({

      name: task.name,

      priority: task.priority,

      status: task.status

    });

  }

  delete(task: Task): void {

    this.successMessage = '';

    this.errorMessage = '';

    this.taskService
      .deleteTask(task.id)
      .subscribe({

        next: () => {

          this.successMessage =
            'Task deleted successfully.';

          this.loadTasks();

        },

        error: error => {

          this.errorMessage =
            getErrorMessage(error);
            this.cdr.detectChanges();

        }

      });

  }

  clear(): void {

    this.successMessage = '';

    this.errorMessage = '';

    this.isEditMode = false;

    this.selectedTaskId = '';

    this.taskForm.reset({

      name: '',

      priority: 1,

      status: TaskStatus.NotStarted

    });

  }

}
