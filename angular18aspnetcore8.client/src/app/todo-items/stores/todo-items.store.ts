import { HttpClient } from '@angular/common/http';
import {
  DeleteTodoItemResponse,
  GetAllTodoItemsResponse,
  TodoItem,
  ValidationErrors,
} from '../models/todo-items-models';
import { makeAutoObservable, runInAction } from 'mobx';
import { Injectable } from '@angular/core';
import { TodoItemStore } from './todo-item.store';

@Injectable({ providedIn: 'root' })
export class TodoItemsStore {
  todoItems: TodoItemStore[] = [];
  actionMessage: string = '';
  actionValidationErrors: ValidationErrors<TodoItem> = {};
  constructor(private httpClient: HttpClient) {
    makeAutoObservable(this);
  }
  setTodoItems(todoItems: TodoItem[], message: string) {
    this.todoItems = todoItems.map(
      (t) => new TodoItemStore(t, this.httpClient)
    );
    this.actionMessage = message;
  }
  getAllTodoItems() {
    this.actionMessage = 'Getting all todo items...';
    this.httpClient.get('/api/todo-items/index').subscribe((result) => {
      const value = result as GetAllTodoItemsResponse;
      const self = this;
      runInAction(() => {
        self.todoItems = value.items.map(
          (t) => new TodoItemStore(t, this.httpClient)
        );
        self.actionMessage = value.message;
      });
    });
  }
  addNew(): TodoItemStore {
    const newTodo: TodoItem = {
      description: '',
      id: 0,
      status: 'To-do',
      dueDate: undefined,
    };
    const newTodoStore = new TodoItemStore(newTodo, this.httpClient);
    this.todoItems = [...this.todoItems, newTodoStore];
    this.actionMessage = 'Adding new todo';
    return newTodoStore;
  }
  delete(toRemove: TodoItemStore) {
    const indexToRemove = this.todoItems.indexOf(toRemove);
    this.httpClient.delete('/api/todo-items/delete').subscribe((result) => {
      const value = result as DeleteTodoItemResponse;
      const self = this;
      runInAction(() => {
        this.todoItems = [
          ...this.todoItems.filter((t, index) => index != indexToRemove),
        ];
        self.actionMessage = value.message;
        self.actionValidationErrors = value.validationErrors;
      });
    });
  }
}
