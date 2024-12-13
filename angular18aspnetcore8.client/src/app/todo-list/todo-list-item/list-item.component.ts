import { Component, Input, OnInit } from '@angular/core';
import { TodoItem } from '../models/todo-list-models';

@Component({
  selector: 'todo-list-item',
  templateUrl: './list-item.component.html',
  styleUrl: './list-item.component.css',
})
export class ListItemComponent implements OnInit {
  @Input({ required: true })
  public item!: TodoItem;

  constructor() {}

  ngOnInit() {}
}
