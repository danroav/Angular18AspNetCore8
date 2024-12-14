import { makeAutoObservable } from 'mobx';
export enum StoreMode {
  view = 0,
  edit = 1,
}
export class TodoItemsStore {
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
  statusValidList: string[] = [];
  statusDefault: string = '';
  constructor() {
    makeAutoObservable(this);
  }
}
