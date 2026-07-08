import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';

import { Task } from '../models/task';
import { CreateTaskRequest } from '../models/create-task-request';
import { UpdateTaskRequest } from '../models/update-task-request';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiUrl}/Tasks`;

  getTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(this.apiUrl);
  }

  getTask(id: string): Observable<Task> {
    return this.http.get<Task>(
      `${this.apiUrl}/${id}`
    );
  }

  createTask(
    request: CreateTaskRequest
  ): Observable<Task> {
    return this.http.post<Task>(
      this.apiUrl,
      request
    );
  }

  updateTask(
    request: UpdateTaskRequest
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${request.id}`,
      request
    );
  }

  deleteTask(
    id: string
  ): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }

}
