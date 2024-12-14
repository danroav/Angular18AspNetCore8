export interface TodoItem {
  id: number;
  description: string;
  dueDate?: string;
  status: string;
}
export interface TodoListIndexResponse {
  count: number;
  message: string;
  items: TodoItem[];
}
export interface CommandAddNewTaskResult {
  taskId: number;
  validationErrors: Record<string, string[]>;
  hasValidationErrors: boolean;
}
