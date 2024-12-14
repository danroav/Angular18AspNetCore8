import { makeAutoObservable } from 'mobx';
export enum StoreMode {
  view = 0,
  edit = 1,
}
export class TodoListItemStore {
  id: number = 0;
  description: string = '';
  status: string = '';
  dueDate?: Date;
  mode: StoreMode = StoreMode.view;
  validations: {
    [property in keyof {
      id?: string;
      description?: string;
      status?: string;
      dueDate?: string;
    }]: string[];
  } = {};
  allowedStatus: string[] = [];
  defaultStatus: string = '';
  constructor() {
    makeAutoObservable(this);
  }
}
