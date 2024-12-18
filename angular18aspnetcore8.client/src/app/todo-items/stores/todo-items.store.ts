import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import {
  DeleteTodoItem,
  DeleteTodoItemResponse,
  GetAllTodoItemsResponse,
  mapTodoItemValidationErrors,
  TodoItem,
  ValidationErrors,
} from '../models/todo-items-models';
import { makeAutoObservable, runInAction } from 'mobx';
import { Injectable } from '@angular/core';
import { TodoItemStore } from './todo-item.store';

@Injectable({ providedIn: 'root' })
export class TodoItemsStore {
  todoItems: TodoItemStore[] = [];
  action: 'setTodoItems' | 'getAllTodoItems' | 'addNew' | 'delete' | undefined =
    undefined;
  actionMessage: string = '';
  actionValidationErrors: ValidationErrors<TodoItem> = {};
  constructor(private httpClient: HttpClient) {
    makeAutoObservable(this);
  }
  setTodoItems(todoItems: TodoItem[], message: string) {
    this.action = 'setTodoItems';
    this.todoItems = todoItems.map(
      (t) => new TodoItemStore(t, this, this.httpClient)
    );
    this.actionMessage = message;
  }
  getAllTodoItems() {
    this.action = 'getAllTodoItems';
    this.actionMessage = 'Getting all todo items...';
    const self = this;
    this.httpClient.get('/api/todo-items/index').subscribe((result) => {
      const value = result as GetAllTodoItemsResponse;
      runInAction(() => {
        self.todoItems = value.items.map(
          (t) => new TodoItemStore(t, this, this.httpClient)
        );
        self.actionMessage = value.message;
      });
    });
  }
  addNew(): TodoItemStore {
    this.action = 'addNew';
    const newTodo: TodoItem = {
      description: '',
      id: 0,
      status: 'To-do',
      dueDate: undefined,
    };
    const newTodoStore = new TodoItemStore(newTodo, this, this.httpClient);
    this.todoItems = [...this.todoItems, newTodoStore];
    this.actionMessage = 'Adding new todo';
    return newTodoStore;
  }
  delete(toRemove: TodoItemStore) {
    this.action = 'delete';
    const expectedTodoItemsAfterDelete = [
      ...this.todoItems.filter((t) => t !== toRemove),
    ];
    if (!this.todoItems.includes(toRemove)) {
      this.actionMessage = 'Todo Item to delete not found';
      this.actionValidationErrors = {};
      return;
    }
    if (toRemove.todoItem.id === 0) {
      this.todoItems = expectedTodoItemsAfterDelete;
      this.actionMessage = 'New todo cancelled';
      this.actionValidationErrors = {};
      return;
    }
    const self = this;
    this.httpClient
      .post<DeleteTodoItemResponse>('/api/todo-items/delete', {
        todoItemId: toRemove.todoItem.id,
      } as DeleteTodoItem)
      .subscribe({
        next: (result) => {
          runInAction(() => {
            self.todoItems = expectedTodoItemsAfterDelete;
            self.actionMessage = result.message;
            self.actionValidationErrors = mapTodoItemValidationErrors(
              result.validationErrors
            );
          });
        },
        error: (error) => {
          runInAction(() => {
            if (error['status'] === 400) {
              const badRequest = error as HttpErrorResponse;
              self.actionMessage = badRequest.error.message;
              self.actionValidationErrors = mapTodoItemValidationErrors(
                badRequest.error.validationErrors
              );
              return;
            }
            self.actionMessage = error.detail;
            self.actionValidationErrors = {};
          });
        },
      });
  }
}
