import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { TodoItem, ValidationErrors } from '../models/todo-items-models';
import { autorun, IReactionDisposer } from 'mobx';
import { TodoItemStore } from '../stores/todo-item.store';
import { FormControl } from '@angular/forms';
import { TodoItemsStore } from '../stores/todo-items.store';

@Component({
  selector: '[todo-item]',
  templateUrl: './todo-item.component.html',
  styleUrl: './todo-item.component.css',
})
export class TodoItemComponent implements OnInit, OnDestroy {
  @Input({ required: true })
  public todoItemStore!: TodoItemStore;
  @Input({ required: true })
  public todoItemsStore!: TodoItemsStore;

  private reactionDisposer?: IReactionDisposer;

  public todoItem: TodoItem = {
    description: '',
    id: 0,
    status: '',
    dueDate: undefined,
  };
  public message: string = '';
  public validationErrors: ValidationErrors<TodoItem> = {};
  public mode: 'view' | 'edit' = 'view';
  formId = new FormControl(0);
  formDescription = new FormControl<string>('');
  formStatus = new FormControl<string>('');
  formDueDate = new FormControl<Date | undefined>(undefined);

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
      this.setFormData();
    });
  }
  setFormData() {
    this.formId.setValue(this.todoItem.id);
    this.formDescription.setValue(this.todoItem.description);
    this.formStatus.setValue(this.todoItem.status);
    this.formDueDate.setValue(this.todoItem.dueDate);
  }
  edit() {
    this.mode = 'edit';
  }
  cancel() {
    this.mode = 'view';
    this.setFormData();
  }
  save() {
    this.todoItemStore.save(
      this.formDescription.value ?? '',
      this.formStatus.value ?? '',
      this.formDueDate.value ?? undefined
    );
    this.mode = 'view';
  }
  delete() {
    this.todoItemsStore.delete(this.todoItemStore);
  }
}
