import { TodoItem, ValidationErrors } from '../models/todo-items-models';

export class TodoItemsStore {
  todoItems: TodoItem[] = [];
  actionMessage: string = '';
  actionTodoItemValidationErrors: {
    [todoItemId: number]: ValidationErrors<TodoItem>;
  } = {};
}
