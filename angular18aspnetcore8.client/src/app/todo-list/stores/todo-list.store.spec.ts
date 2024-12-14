import { StoreMode, TodoListItemStore } from './todo-list.store';

describe('Todo List Item Store', () => {
  let testTodoListItemStore: TodoListItemStore;
  beforeEach(() => {});
  it('When creating new instance', () => {
    //Arrange
    const givenId = 1;
    const givenDescription = 'some description';
    const givenStatus = 'some status';
    const givenDueDate = undefined;
    //Act
    testTodoListItemStore = new TodoListItemStore(
      givenId,
      givenDescription,
      givenStatus,
      givenDueDate
    );
    //Assert
    expect(testTodoListItemStore.description).toEqual(givenDescription);
    expect(testTodoListItemStore.dueDate).toEqual(givenDueDate);
    expect(testTodoListItemStore.id).toEqual(givenId);
    expect(testTodoListItemStore.mode).toEqual(StoreMode.view);
    expect(testTodoListItemStore.status).toEqual(givenStatus);
    expect(testTodoListItemStore.validations).toEqual({});
  });
});
