import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { TodoItem, ValidationErrors } from '../models/todo-items-models';
import { autorun, IReactionDisposer } from 'mobx';
import { TodoItemStore } from '../stores/todo-item.store';

@Component({
  selector: '[todo-item]',
  templateUrl: './todo-item.component.html',
  styleUrl: './todo-item.component.css',
})
export class TodoItemComponent implements OnInit, OnDestroy {
  @Input({ required: true })
  public todoItemStore!: TodoItemStore;
  private reactionDisposer?: IReactionDisposer;

  public todoItem: TodoItem = {
    description: '',
    id: 0,
    status: '',
    dueDate: undefined,
  };
  public message: string = '';
  public validationErrors: ValidationErrors<TodoItem> = {};

  constructor() {}
  ngOnDestroy(): void {
    if (this.reactionDisposer) {
      this.reactionDisposer!();
    }
  }

  ngOnInit() {
    this.reactionDisposer = autorun(() => {
      this.todoItem = this.todoItemStore.todoItem;
      this.message = this.todoItemStore.actionMessage;
      this.validationErrors = this.todoItemStore.actionValidationErrors;
    });
  }
}
