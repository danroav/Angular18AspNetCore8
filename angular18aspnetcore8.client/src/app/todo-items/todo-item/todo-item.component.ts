import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import {
  StoreMode,
  TodoItem,
  ValidationErrors,
} from '../models/todo-items-models';
import { autorun, IReactionDisposer } from 'mobx';
import { TodoItemStore } from '../stores/todo-item.store';
import { FormControl, FormGroup } from '@angular/forms';
import { TodoItemsStore } from '../stores/todo-items.store';

@Component({
  selector: '[todo-item]',
  templateUrl: './todo-item.component.html',
  styleUrl: './todo-item.component.css',
})
export class TodoItemComponent implements OnInit, OnDestroy {
  @Input({ required: true })
  todoItemStore!: TodoItemStore;
  @Input({ required: true })
  todoItemsStore!: TodoItemsStore;

  todoItem: TodoItem = {
    description: '',
    id: 0,
    status: '',
    dueDate: undefined,
  };
  actionMessage: string = '';
  actionValidationErrors: ValidationErrors<TodoItem> = {};
  mode: StoreMode = 'view';

  formGroup = new FormGroup({
    id: new FormControl<number | undefined>(0),
    description: new FormControl<string | undefined>(''),
    status: new FormControl<string | undefined>(''),
    dueDate: new FormControl<Date | undefined>(undefined),
  });
  private reactionDisposers: IReactionDisposer[] = [];

  constructor() {}
  ngOnDestroy(): void {
    for (let reactionDisposer of this.reactionDisposers) {
      reactionDisposer();
    }
  }

  ngOnInit() {
    this.reactionDisposers.push(
      autorun(() => {
        this.mode = this.todoItemStore.mode;
      })
    );
    this.reactionDisposers.push(
      autorun(() => {
        this.todoItem = this.todoItemStore.todoItem;
        this.formGroup.setValue(this.todoItem as any);
      })
    );
    this.reactionDisposers.push(
      autorun(() => {
        this.actionMessage = this.todoItemStore.actionMessage;
        this.actionValidationErrors = this.todoItemStore.actionValidationErrors;
        this.formGroup.setErrors(null);
        for (let [key, value] of Object.entries(this.actionValidationErrors)) {
          this.formGroup.get(key)?.setErrors({ error: value });
        }
      })
    );
  }
  edit() {
    this.todoItemStore.startEdit();
  }
  cancel() {
    this.todoItemStore.cancelEdit();
  }
  save() {
    this.todoItemStore.save(
      this.formGroup.value.description ?? '',
      this.formGroup.value.status ?? '',
      this.formGroup.value.dueDate ?? undefined
    );
  }
  delete() {
    this.todoItemStore.delete();
  }
}
