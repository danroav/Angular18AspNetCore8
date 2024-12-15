import { TodoItemsStore } from './todo-items.store';

describe('Todo Items Store', () => {
  let testTodoItemsStore: TodoItemsStore;
  beforeEach(() => {
    testTodoItemsStore = new TodoItemsStore();
  });
  it('Have default values', () => {
    //Arrange
    //Act
    //Assert
    expect(testTodoItemsStore.todoItems).toEqual([]);
    expect(testTodoItemsStore.actionMessage).toEqual('');
    expect(testTodoItemsStore.actionTodoItemValidationErrors).toEqual({});
  });
});
