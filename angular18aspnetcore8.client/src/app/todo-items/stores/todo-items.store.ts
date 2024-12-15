import { HttpClient } from '@angular/common/http';
import { makeAutoObservable, runInAction } from 'mobx';
import {
  CreateTodoItem as AddNewTodoItem,
  CreateTodoItemResult,
  TodoItem,
  ValidationErrors,
} from '../models/todo-items-models';

export class TodoItemStore {
  id: number = 0;
  description: string = '';
  status: string = '';
  dueDate?: Date;
  validationErrors: ValidationErrors<TodoItem> = {};
  message: string = '';

  constructor(private httpClient: HttpClient) {
    makeAutoObservable(this);
  }

  create(description: string, status: string, dueDate?: Date) {
    const requestBody: AddNewTodoItem = {
      description,
      status,
      dueDate,
    };
    const self = this;
    this.httpClient.post('/api/todo-items/create', requestBody).subscribe({
      next: (value) => {
        runInAction(() => {
          const result = value as CreateTodoItemResult;
          self.id = result.item.id;
          self.description = result.item.description;
          self.status = result.item.status;
          self.dueDate = result.item.dueDate;
          self.validationErrors = result.validationErrors;
          self.message = result.message;
        });
      },
      error: (error) => {
        runInAction(() => {
          self.message = error.detail;
          self.validationErrors = {};
        });
      },
    });
  }
}
