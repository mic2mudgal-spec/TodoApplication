import { TaskStatus } from './task-status';

export interface UpdateTaskRequest {
  id: string;
  name: string;
  priority: number;
  status: TaskStatus;
}