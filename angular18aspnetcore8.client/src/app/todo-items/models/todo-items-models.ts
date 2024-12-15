export interface TodoItem {
  id: number;
  description: string;
  dueDate?: Date;
  status: string;
}
export interface TodoItemsIndexResponse {
  count: number;
  message: string;
  items: TodoItem[];
}
export interface CreateTodoItem {
  description: string;
  dueDate?: Date;
  status: string;
}
export interface CreateTodoItemResult {
  item: TodoItem;
  validationErrors: Record<string, string[]>;
  message: string;
}
