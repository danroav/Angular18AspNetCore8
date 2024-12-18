export function mapTodoItemValidationErrors(
  validationErrors: ValidationErrors<{ [key: string]: string[] }>
) {
  const result: ValidationErrors<TodoItem> = {};
  if (Object.hasOwn(validationErrors, 'Item.Id')) {
    result.id = validationErrors['Item.Id'];
  }
  if (Object.hasOwn(validationErrors, 'Item.Description')) {
    result.description = validationErrors['Item.Description'];
  }
  if (Object.hasOwn(validationErrors, 'Item.DueDate')) {
    result.dueDate = validationErrors['Item.DueDate'];
  }
  if (Object.hasOwn(validationErrors, 'Item.Status')) {
    result.status = validationErrors['Item.Status'];
  }
  return result;
}

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
  validationErrors: ValidationErrors<{ [key: string]: string[] }>;
  message: string;
}
export interface UpdateTodoItem {
  item: TodoItem;
}
export interface UpdateTodoItemResponse {
  item: TodoItem;
  validationErrors: ValidationErrors<{ [key: string]: string[] }>;
  message: string;
}

export interface DeleteTodoItem {
  todoItemId: number;
}
export interface DeleteTodoItemResponse {
  item?: TodoItem;
  validationErrors: ValidationErrors<{ [key: string]: string[] }>;
  message: string;
}
