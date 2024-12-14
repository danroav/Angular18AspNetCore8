export interface TodoItem {
  id: number;
  description: string;
  dueDate?: string;
  status: string;
}
export interface TodoItemsIndexResponse {
  count: number;
  message: string;
  items: TodoItem[];
}
export interface CommandAddNewTodoItemResponse {
  item: TodoItem;
  validationErrors: Record<string, string[]>;
  hasValidationErrors: boolean;
}
