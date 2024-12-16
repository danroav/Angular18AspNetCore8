import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import {
  CreateTodoItem,
  CreateTodoItemResponse,
  GetAllTodoItemsResponse,
  TodoItem,
} from './models/todo-items-models';

@Component({
  selector: 'todo-items',
  templateUrl: './todo-items.component.html',
  styleUrl: './todo-items.component.css',
})
export class TodoItemsComponent implements OnInit {
  public items: TodoItem[] = [];
  public message: string = '';
  title = 'Todo Items';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getAllTodoItems();
  }

  getAllTodoItems() {
    this.http.get<GetAllTodoItemsResponse>('/api/todo-items/index').subscribe({
      next: (result) => {
        this.items = result.items;
        this.message = result.message;
      },
      error: (error) => {
        this.message = error.error.detail;
        this.items = [];
      },
    });
  }
  create(createTodoItem: CreateTodoItem) {
    this.http
      .post<CreateTodoItemResponse>('/api/todo-items/create', createTodoItem)
      .subscribe({
        next: (result) => {
          this.items = [...this.items, result.item];
          this.message = result.message;
        },
        error: (error) => {
          this.message = error.error.detail;
        },
      });
  }
}
