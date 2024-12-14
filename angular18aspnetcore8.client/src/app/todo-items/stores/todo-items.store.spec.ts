import { StoreMode, TodoItemsStore } from './todo-items.store';

describe('Todo Items Store', () => {
  let testStore: TodoItemsStore;

  it('When creating new instance', () => {
    //Arrange
    //Act
    testStore = new TodoItemsStore();
    //Assert
    expect(testStore.description).toEqual('');
    expect(testStore.dueDate).not.toBeDefined();
    expect(testStore.id).toEqual(0);
    expect(testStore.mode).toEqual(StoreMode.view);
    expect(testStore.status).toEqual('');
    expect(testStore.validations).toEqual({});
    expect(testStore.statusValidList).toEqual([]);
    expect(testStore.statusDefault).toEqual('');
  });
  /*
  it('When adding new item success', () => {
    //Arrange
    const givenId = 0;
    const givenDescription = 'some description';
    const givenStatus = 'some status';
    const givenDueDate = undefined;

    testTodoListItemStore = new TodoListItemStore();
    const givenCommandAddNewTaskResult: CommandAddNewTaskResult = {
      hasValidationErrors: false,
      taskId: 14,
      validationErrors: {},
    };

    const mockHttpClient: HttpClient = {
      get: jasmine.createSpy().and.returnValue(givenCommandAddNewTaskResult),
    } as any;

    //Act
    testTodoListItemStore.saveNew(givenDescription, givenDueDate, givenStatus);

    //Assert
    expect(mockHttpClient.get).toHaveBeenCalledWith('/api/todo-list/index');
    expect(testTodoListItemStore.description).toEqual();
    expect(testTodoListItemStore.dueDate);
  });
  */
});
