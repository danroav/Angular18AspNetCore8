export interface TodoItem {
  id: number;
  description: string;
  dueDate?: Date;
  status: string;
}
export type ValidationErrors<T> = {
  [property in keyof T]?: string[];
};

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
  validationErrors: ValidationErrors<TodoItem>;
  message: string;
}
