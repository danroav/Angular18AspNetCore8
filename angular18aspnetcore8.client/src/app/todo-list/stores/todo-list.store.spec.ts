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
    expect(testTodoListItemStore.statusValidList).toEqual([]);
    expect(testTodoListItemStore.statusDefault).toEqual('');
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
