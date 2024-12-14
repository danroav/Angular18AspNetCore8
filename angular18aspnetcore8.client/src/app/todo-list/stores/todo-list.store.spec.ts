import { StoreMode, TodoListItemStore } from './todo-list.store';

describe('Todo List Item Store', () => {
  let testTodoListItemStore: TodoListItemStore;

  it('When creating new instance', () => {
    //Arrange
    //Act
    testTodoListItemStore = new TodoListItemStore();
    //Assert
    expect(testTodoListItemStore.description).toEqual('');
    expect(testTodoListItemStore.dueDate).not.toBeDefined();
    expect(testTodoListItemStore.id).toEqual(0);
    expect(testTodoListItemStore.mode).toEqual(StoreMode.view);
    expect(testTodoListItemStore.status).toEqual('');
    expect(testTodoListItemStore.validations).toEqual({});
    expect(testTodoListItemStore.allowedStatus).toEqual([]);
    expect(testTodoListItemStore.defaultStatus).toEqual('');
  });
});
