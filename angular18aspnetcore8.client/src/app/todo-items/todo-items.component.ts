import { Component, OnDestroy, OnInit } from '@angular/core';
import { TodoItemsStore } from './stores/todo-items.store';
import { autorun, IReactionDisposer } from 'mobx';
import { TodoItemStore } from './stores/todo-item.store';
import { TodoItem, ValidationErrors } from './models/todo-items-models';

@Component({
  selector: 'todo-items',
  templateUrl: './todo-items.component.html',
  styleUrl: './todo-items.component.css',
})
export class TodoItemsComponent implements OnInit, OnDestroy {
  title = 'Todo Items';
  public todoItemStoreList: TodoItemStore[] = [];
  public message: string = '';
  public validationErrors: ValidationErrors<TodoItem> = {};

  private reactionDisposer?: IReactionDisposer;

  constructor(public todoItemsStore: TodoItemsStore) {}

  ngOnDestroy(): void {
    if (this.reactionDisposer) {
      this.reactionDisposer!();
    }
  }

  ngOnInit() {
    this.reactionDisposer = autorun(() => {
      this.todoItemStoreList = this.todoItemsStore.todoItems;
      this.message = this.todoItemsStore.actionMessage;
      this.validationErrors = this.todoItemsStore.actionValidationErrors;
    });
    this.todoItemsStore.getAllTodoItems();
  }
}
