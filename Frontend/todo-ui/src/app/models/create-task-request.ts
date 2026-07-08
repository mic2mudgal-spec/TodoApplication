import { TaskStatus } from './task-status';

export interface CreateTaskRequest {
  name: string;
  priority: number;
  status: TaskStatus;
}