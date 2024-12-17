import { HttpClient } from '@angular/common/http';
import { makeAutoObservable, runInAction } from 'mobx';
import {
  CreateTodoItem,
  CreateTodoItemResponse,
  StoreMode,
  TodoItem,
  UpdateTodoItem,
  UpdateTodoItemResponse,
  ValidationErrors,
} from '../models/todo-items-models';
export class TodoItemStore {
  mode: StoreMode = 'view';
  actionMessage: string = '';
  actionValidationErrors: ValidationErrors<TodoItem> = {};
  todoItem: TodoItem;
  constructor(public givenTodoItem: TodoItem, private httpClient: HttpClient) {
    this.todoItem = givenTodoItem;
    makeAutoObservable(this);
  }
  setMode(mode: StoreMode) {
    this.mode = mode;
  }
  save(description: string, status: string, dueDate?: Date) {
    const self = this;
    if (this.todoItem.id === 0) {
      const newTodoPostBody: CreateTodoItem = {
        description: description,
        status: status,
        dueDate: dueDate,
      };
      this.httpClient
        .post('/api/todo-items/create', newTodoPostBody)
        .subscribe({
          next: (result) => {
            console.log('next', result);
            const value = result as CreateTodoItemResponse;
            runInAction(() => {
              if (Object.keys(value.validationErrors).length === 0) {
                self.todoItem = value.item;
              }
              self.actionMessage = value.message;
              self.actionValidationErrors = value.validationErrors;
              self.mode = 'view';
            });
          },
          error: (error) => {
            console.log('error', error);
            runInAction(() => {
              self.actionMessage = error.detail;
              self.actionValidationErrors = {};
              self.mode = 'edit';
            });
          },
        });
      return;
    }
    const updateTodoPostBody: UpdateTodoItem = {
      item: {
        id: self.todoItem.id,
        description: description,
        status: status,
        dueDate: dueDate,
      },
    };
    this.httpClient
      .post('/api/todo-items/update', updateTodoPostBody)
      .subscribe({
        next: (result) => {
          const value = result as UpdateTodoItemResponse;
          runInAction(() => {
            if (Object.keys(value.validationErrors).length === 0) {
              self.todoItem = value.item;
            }
            self.actionMessage = value.message;
            self.actionValidationErrors = value.validationErrors;
            self.mode = 'view';
          });
        },
        error: (error) => {
          runInAction(() => {
            self.actionMessage = error.detail;
            self.actionValidationErrors = {};
            self.mode = 'edit';
          });
        },
      });
  }
}
