import { Component, OnDestroy, OnInit } from '@angular/core';
import { TodoItemsStore } from './stores/todo-items.store';
import { autorun, IReactionDisposer } from 'mobx';
import { TodoItemStore } from './stores/todo-item.store';

@Component({
  selector: 'todo-items',
  templateUrl: './todo-items.component.html',
  styleUrl: './todo-items.component.css',
})
export class TodoItemsComponent implements OnInit, OnDestroy {
  public todoItemStoreList: TodoItemStore[] = [];
  public message: string = '';
  title = 'Todo Items';
  private reactionDisposer?: IReactionDisposer;
  constructor(private todoItemsStore: TodoItemsStore) {}
  ngOnDestroy(): void {
    if (this.reactionDisposer) {
      this.reactionDisposer!();
    }
  }

  ngOnInit() {
    this.reactionDisposer = autorun(() => {
      this.todoItemStoreList = this.todoItemsStore.todoItems;
      this.message = this.todoItemsStore.actionMessage;
    });
    this.todoItemsStore.getAllTodoItems();
  }
}
