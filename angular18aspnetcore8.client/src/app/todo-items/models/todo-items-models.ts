export type ValidationErrors<T> = {
  [property in keyof T]?: string[];
};
export type StoreMode = 'edit' | 'view';
export interface TodoItem {
  id: number;
  description: string;
  dueDate?: Date;
  status: string;
}
export interface GetAllTodoItemsResponse {
  message: string;
  items: TodoItem[];
}
export interface CreateTodoItem {
  description: string;
  dueDate?: Date;
  status: string;
}
export interface CreateTodoItemResponse {
  item: TodoItem;
  validationErrors: ValidationErrors<TodoItem>;
  message: string;
}
export interface UpdateTodoItem {
  item: TodoItem;
}
export interface UpdateTodoItemResponse {
  item: TodoItem;
  validationErrors: ValidationErrors<TodoItem>;
  message: string;
}

export interface DeleteTodoItem {
  todoItemId: number;
}
export interface DeleteTodoItemResponse {
  item?: TodoItem;
  validationErrors: ValidationErrors<TodoItem>;
  message: string;
}
