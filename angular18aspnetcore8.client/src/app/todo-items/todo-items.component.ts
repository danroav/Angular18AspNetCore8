import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { TodoItem, TodoItemsIndexResponse } from './models/todo-items-models';

@Component({
  selector: 'todo-items',
  templateUrl: './todo-items.component.html',
  styleUrl: './todo-items.component.css',
})
export class TodoItemsComponent implements OnInit {
  public items: TodoItem[] = [];
  public message: string = '';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getTodoItems();
  }

  getTodoItems() {
    this.http.get<TodoItemsIndexResponse>('/api/todo-items/index').subscribe({
      next: (result) => {
        this.items = result.items;
        this.message = result.message;
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  title = 'Todo Items';
}
