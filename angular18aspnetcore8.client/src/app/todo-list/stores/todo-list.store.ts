import { makeAutoObservable } from 'mobx';
export enum StoreMode {
  view = 0,
  edit = 1,
}
export class TodoListItemStore {
  mode: StoreMode = StoreMode.view;
  validations: {
    [property in keyof {
      id?: string;
      description?: string;
      status?: string;
      dueDate?: string;
    }]: string[];
  } = {};

  constructor(
    public id: number,
    public description: string,
    public status: string,
    public dueDate?: Date
  ) {
    makeAutoObservable(this);
  }
}
