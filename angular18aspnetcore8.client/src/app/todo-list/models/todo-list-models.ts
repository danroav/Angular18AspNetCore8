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
