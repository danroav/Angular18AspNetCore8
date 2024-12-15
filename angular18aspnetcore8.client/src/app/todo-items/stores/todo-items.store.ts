import { HttpClient } from '@angular/common/http';
import {
  CreateTodoItemResult,
  GetAllTodoItemsResult,
  TodoItem,
  ValidationErrors,
} from '../models/todo-items-models';
import { makeAutoObservable, runInAction } from 'mobx';

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
      const value = result as GetAllTodoItemsResult;
      const self = this;
      runInAction(() => {
        self.todoItems = value.todoItems;
        self.actionMessage = value.message;
      });
    });
  }
}
