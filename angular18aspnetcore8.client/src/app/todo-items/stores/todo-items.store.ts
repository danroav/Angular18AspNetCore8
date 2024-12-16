import { HttpClient } from '@angular/common/http';
import { GetAllTodoItemsResponse, TodoItem } from '../models/todo-items-models';
import { makeAutoObservable, runInAction } from 'mobx';
import { Injectable } from '@angular/core';
import { TodoItemStore } from './todo-item.store';

@Injectable({ providedIn: 'root' })
export class TodoItemsStore {
  todoItems: TodoItemStore[] = [];
  actionMessage: string = '';
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
  addNew() {
    const newTodo: TodoItem = {
      description: '',
      id: 0,
      status: 'To-do',
      dueDate: undefined,
    };
    this.todoItems = [
      ...this.todoItems,
      new TodoItemStore(newTodo, this.httpClient),
    ];
    this.actionMessage = 'Adding new todo';
  }
}
