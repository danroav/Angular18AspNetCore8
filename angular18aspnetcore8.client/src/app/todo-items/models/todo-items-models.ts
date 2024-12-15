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
export interface AddnewTodoItem {
  description: string;
  dueDate?: Date;
  status: string;
}
export interface AddNewTodoItemResult {
  item: TodoItem;
  validationErrors: Record<string, string[]>;
  hasValidationErrors: boolean;
}
