import { HttpClient } from '@angular/common/http';
import {
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
