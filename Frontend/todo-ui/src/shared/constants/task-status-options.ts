import { TaskStatus } from '../../app/models/task-status';

export const TASK_STATUS_OPTIONS = [
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
