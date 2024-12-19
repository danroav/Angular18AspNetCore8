import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import {
  StoreMode,
  TodoItem,
  ValidationErrors,
} from '../models/todo-items-models';
import { autorun, IReactionDisposer, toJS } from 'mobx';
import { TodoItemStore } from '../stores/todo-item.store';
import { FormControl, FormGroup } from '@angular/forms';
import { formatDate } from '@angular/common';

@Component({
  selector: '[todo-item]',
  templateUrl: './todo-item.component.html',
  styleUrl: './todo-item.component.css',
})
export class TodoItemComponent implements OnInit, OnDestroy {
  @Input({ required: true })
  todoItemStore!: TodoItemStore;

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
    dueDateRead: new FormControl<string | undefined>(''),
  });
  private readonly reactionDisposers: IReactionDisposer[] = [];

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
        this.formGroup.setValue({
          ...(this.todoItem as any),
          dueDateRead: this.todoItem.dueDate
            ? formatDate(this.todoItem.dueDate, 'yyyy-MM-dd', 'en', '-0400')
            : '',
        });
      })
    );
    this.reactionDisposers.push(
      autorun(() => {
        this.actionMessage = this.todoItemStore.actionMessage;
        this.actionValidationErrors = this.todoItemStore.actionValidationErrors;
        this.formGroup.setErrors(null);
        for (let [key, value] of Object.entries(
          toJS(this.actionValidationErrors)
        )) {
          this.formGroup.get(key)?.setErrors({ error: value.join('. ') });
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
    console.log('formgroup', this.formGroup);

    this.todoItemStore.save(
      this.formGroup.value.description ?? '',
      this.formGroup.value.status ?? '',
      this.formGroup.value.dueDateRead
        ? new Date(this.formGroup.value.dueDateRead! + 'T00:00:00-04:00')
        : undefined
    );
  }
  delete() {
    this.todoItemStore.delete();
  }
}
