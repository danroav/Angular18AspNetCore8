import { Component, Input, OnInit } from '@angular/core';
import { TodoItem } from '../models/todo-items-models';

@Component({
  selector: '[todo-item]',
  templateUrl: './todo-item.component.html',
  styleUrl: './todo-item.component.css',
})
export class TodoItemComponent implements OnInit {
  @Input({ required: true })
  public item!: TodoItem;

  constructor() {}

  ngOnInit() {}
}
