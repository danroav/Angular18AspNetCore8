import { HttpClient } from '@angular/common/http';
import { TodoItemsStore, TodoItemStore } from './todo-items.store';
import { reaction } from 'mobx';
import { GetAllTodoItemsResponse, TodoItem } from '../models/todo-items-models';
import { of } from 'rxjs';

describe('Todo Items Store', () => {
  let testTodoItemsStore: TodoItemsStore;
  const spyHttpClientGet = jasmine.createSpy('httpClientGet');
  const mockHttpClient: HttpClient = {
    get: spyHttpClientGet,
  } as any;
  beforeEach(() => {
    testTodoItemsStore = new TodoItemsStore(mockHttpClient);
  });
  it('Have default values', () => {
    //Arrange
    //Act
    //Assert
    expect(testTodoItemsStore.todoItems).toEqual([]);
    expect(testTodoItemsStore.actionMessage).toEqual('');
    expect(testTodoItemsStore.actionTodoItemValidationErrors).toEqual({});
  });
  describe('Get all todo items', () => {
    it('Should get items from backend', async () => {
      //Arrange
      const givenResponseTodoItems: TodoItem[] = [];
      const givenResponseMessage = '';
      const expectedResponseValidationErrors = {};

      const givenGetAllTodoItemsResponse: GetAllTodoItemsResponse = {
        message: givenResponseMessage,
        todoItems: givenResponseTodoItems,
      };
      spyHttpClientGet.and.returnValue(of(givenGetAllTodoItemsResponse));

      const changePromise = new Promise<void>((resolve, reject) => {
        reaction(
          () => testTodoItemsStore.todoItems,
          (_arg, _prev, r) => {
            try {
              expect(testTodoItemsStore.todoItems).toEqual(
                givenResponseTodoItems
              );
              expect(testTodoItemsStore.actionMessage).toEqual(
                givenResponseMessage
              );
              expect(testTodoItemsStore.actionTodoItemValidationErrors).toEqual(
                expectedResponseValidationErrors
              );
              resolve();
            } catch (error) {
              reject();
            } finally {
              r.dispose();
            }
          }
        );
      });
      //Act
      testTodoItemsStore.getAllTodoItems();
      //Assert
      testTodoItemsStore.actionMessage = 'Getting all todo items...';
      return changePromise;
    });
  });
});
describe('Todo Item store', () => {
  let testTodoItemStore: TodoItemStore;
  const givenTodoItem: TodoItem = {
    id: 0,
    description: 'given todo item',
    status: 'given status',
    dueDate: new Date(),
  };
  const spyHttpClientGet = jasmine.createSpy('httpClientGet');
  const mockHttpClient: HttpClient = {
    get: spyHttpClientGet,
  } as any;
  beforeEach(() => {
    testTodoItemStore = new TodoItemStore(givenTodoItem, mockHttpClient);
  });
  it('Have default values', () => {
    //Arrange
    //Act
    //Assert
    expect(testTodoItemStore.todoItem).toEqual(givenTodoItem);
    expect(testTodoItemStore.actionMessage).toEqual('');
    expect(testTodoItemStore.actionValidationErrors).toEqual({});
  });
});
