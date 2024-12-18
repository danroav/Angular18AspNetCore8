import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { makeAutoObservable } from 'mobx';
import {
  CreateTodoItem,
  CreateTodoItemResponse,
  mapTodoItemValidationErrors,
  StoreMode,
  TodoItem,
  UpdateTodoItem,
  UpdateTodoItemResponse,
  ValidationErrors,
} from '../models/todo-items-models';
import { TodoItemsStore } from './todo-items.store';
export class TodoItemStore {
  mode: StoreMode = 'view';
  actionMessage: string = '';
  actionValidationErrors: ValidationErrors<TodoItem> = {};
  todoItem: TodoItem;
  constructor(
    public givenTodoItem: TodoItem,
    private todoItemsStore: TodoItemsStore,
    private httpClient: HttpClient
  ) {
    this.todoItem = givenTodoItem;
    makeAutoObservable(this);
  }
  handleCreateResponse(result: CreateTodoItemResponse) {
    if (Object.keys(result.validationErrors).length === 0) {
      this.todoItem = result.item;
    }
    this.actionMessage = result.message;
    this.actionValidationErrors = mapTodoItemValidationErrors(
      result.validationErrors
    );
    this.mode = 'view';
  }
  handleUpdateResponse(result: UpdateTodoItemResponse) {
    if (Object.keys(result.validationErrors).length === 0) {
      this.todoItem = result.item;
    }
    this.actionMessage = result.message;
    this.actionValidationErrors = mapTodoItemValidationErrors(
      result.validationErrors
    );
    this.mode = 'view';
  }
  handleErrorResponse(error: any) {
    if (error['status'] === 400) {
      const badRequest = error as HttpErrorResponse;
      this.actionMessage = badRequest.error.message;
      this.actionValidationErrors = mapTodoItemValidationErrors(
        badRequest.error.validationErrors
      );
      this.mode = 'edit';
      return;
    }
    this.actionMessage = error.detail;
    this.actionValidationErrors = {};
    this.mode = 'edit';
  }
  save(description: string, status: string, dueDate?: Date) {
    if (this.todoItem.id === 0) {
      const newTodoPostBody: CreateTodoItem = {
        description: description,
        status: status,
        dueDate: dueDate,
      };
      this.httpClient
        .post<CreateTodoItemResponse>('/api/todo-items/create', newTodoPostBody)
        .subscribe({
          next: (result) => {
            this.handleCreateResponse(result);
          },
          error: (error) => {
            this.handleErrorResponse(error);
          },
        });
      return;
    }
    const updateTodoPostBody: UpdateTodoItem = {
      item: {
        id: this.todoItem.id,
        description: description,
        status: status,
        dueDate: dueDate,
      },
    };
    this.httpClient
      .post<UpdateTodoItemResponse>(
        '/api/todo-items/update',
        updateTodoPostBody
      )
      .subscribe({
        next: (result) => {
          this.handleUpdateResponse(result);
        },
        error: (error) => {
          this.handleErrorResponse(error);
        },
      });
  }
  startEdit() {
    this.mode = 'edit';
  }
  cancelEdit() {
    if (this.todoItem.id === 0) {
      this.delete();
      return;
    }
    this.todoItem = this.givenTodoItem;
    this.mode = 'view';
  }
  delete() {
    this.todoItemsStore.delete(this);
  }
}
