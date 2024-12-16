export interface TodoItem {
  id: number;
  description: string;
  dueDate?: Date;
  status: string;
}
export type ValidationErrors<T> = {
  [property in keyof T]?: string[];
};
export interface GetAllTodoItemsResponse {
  message: string;
  todoItems: TodoItem[];
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
