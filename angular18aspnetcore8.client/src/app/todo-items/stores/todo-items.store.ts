import { HttpClient } from '@angular/common/http';
import {
  CreateTodoItem,
  CreateTodoItemResponse,
  GetAllTodoItemsResponse,
  TodoItem,
  ValidationErrors,
} from '../models/todo-items-models';
import { makeAutoObservable, runInAction } from 'mobx';

export class TodoItemStore {
  actionMessage: string = '';
  actionValidationErrors: ValidationErrors<TodoItem> = {};
  constructor(public todoItem: TodoItem, private httpClient: HttpClient) {
    makeAutoObservable(this);
  }
  save(description: string, status: string, dueDate?: Date) {
    const self = this;
    if (this.todoItem.id === 0) {
      const postBody: CreateTodoItem = {
        description: description,
        status: status,
        dueDate: dueDate,
      };
      this.httpClient.post('/api/todo-items/create', postBody).subscribe({
        next: (result) => {
          const value = result as CreateTodoItemResponse;
          runInAction(() => {
            if (Object.keys(value.validationErrors).length === 0) {
              self.todoItem = value.item;
            }
            self.actionMessage = value.message;
            self.actionValidationErrors = value.validationErrors;
          });
        },
        error: (error) => {
          runInAction(() => {
            self.actionMessage = error.detail;
            self.actionValidationErrors = {};
          });
        },
      });
    }
  }
}
export class TodoItemsStore {
  todoItems: TodoItem[] = [];
  actionMessage: string = '';
  actionTodoItemValidationErrors: {
    [todoItemId: number]: ValidationErrors<TodoItem>;
  } = {};
  constructor(private httpClient: HttpClient) {
    makeAutoObservable(this);
  }
  getAllTodoItems() {
    this.actionMessage = 'Getting all todo items...';
    this.httpClient.get('/api/todo-items/index').subscribe((result) => {
      const value = result as GetAllTodoItemsResponse;
      const self = this;
      runInAction(() => {
        self.todoItems = value.todoItems;
        self.actionMessage = value.message;
      });
    });
  }
}
