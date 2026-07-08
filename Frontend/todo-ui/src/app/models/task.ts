import { TaskStatus } from './task-status';

export interface Task {

  id: string;

  name: string;

  priority: number;

  status: TaskStatus;

}
