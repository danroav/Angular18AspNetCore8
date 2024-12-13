import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { TodoItem, TodoListIndexResponse } from './models/todo-list-models';

@Component({
  selector: 'todo-list',
  templateUrl: './list.component.html',
  styleUrl: './list.component.css',
})
export class ListComponent implements OnInit {
  public items: TodoItem[] = [];
  public message: string = '';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getItems();
  }

  getItems() {
    this.http.get<TodoListIndexResponse>('/api/todo-list/index').subscribe({
      next: (result) => {
        this.items = result.items;
        this.message = result.message;
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  title = 'Todo List';
}