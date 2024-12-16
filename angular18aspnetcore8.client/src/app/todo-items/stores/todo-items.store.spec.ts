import { HttpClient } from '@angular/common/http';
import { TodoItemsStore } from './todo-items.store';
import { reaction } from 'mobx';
import {
  DeleteTodoItem,
  DeleteTodoItemResponse,
  GetAllTodoItemsResponse,
  TodoItem,
} from '../models/todo-items-models';
import { Observable, of } from 'rxjs';
import { TodoItemStore } from './todo-item.store';

describe('Todo Items Store', () => {
  let testTodoItemsStore: TodoItemsStore;
  const spyHttpClientGet = jasmine.createSpy('httpClientGet');
  const spyHttpClientDelete = jasmine.createSpy('httpClientDelete');
  const mockHttpClient: HttpClient = {
    get: spyHttpClientGet,
    delete: spyHttpClientDelete,
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
  });
  describe('Get all todo items', () => {
    it('Should get items from backend', async () => {
      //Arrange
      const givenResponseTodoItems: TodoItem[] = [];
      const givenResponseMessage = '';

      const givenGetAllTodoItemsResponse: GetAllTodoItemsResponse = {
        message: givenResponseMessage,
        items: givenResponseTodoItems,
      };
      spyHttpClientGet.and.returnValue(of(givenGetAllTodoItemsResponse));

      const changePromise = new Promise<void>((resolve, reject) => {
        reaction(
          () => testTodoItemsStore.todoItems,
          (_arg, _prev, r) => {
            try {
              expect(testTodoItemsStore.todoItems).toEqual(
                givenResponseTodoItems.map(
                  (t) => new TodoItemStore(t, mockHttpClient)
                )
              );
              expect(testTodoItemsStore.actionMessage).toEqual(
                givenResponseMessage
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

  describe('Add new todo', async () => {
    it('should add new todo to list', () => {
      //Arrange
      const givenTodoItems: TodoItem[] = [
        {
          id: 1,
          description: 'given description 1',
          status: 'given status 1',
          dueDate: new Date(),
        },
        {
          id: 2,
          description: 'given description 2',
          status: 'given status 2',
          dueDate: new Date(),
        },
      ];
      const givenMessage = 'given message';
      const expectedNewTodoItem: TodoItem = {
        id: 0,
        description: '',
        status: 'To-do',
        dueDate: undefined,
      };
      const givenHttpClient: HttpClient = {} as any;
      testTodoItemsStore = new TodoItemsStore(givenHttpClient);
      testTodoItemsStore.setTodoItems(givenTodoItems, givenMessage);
      const changePromise = new Promise<void>((resolve, reject) => {
        reaction(
          () => ({
            r1: testTodoItemsStore.todoItems,
            r2: testTodoItemsStore.actionMessage,
          }),
          (_arg, _prev, r) => {
            try {
              expect(_arg).toEqual({
                r1: [
                  ...givenTodoItems.map(
                    (t) => new TodoItemStore(t, givenHttpClient)
                  ),
                  new TodoItemStore(expectedNewTodoItem, givenHttpClient),
                ],
                r2: 'Adding new todo',
              });
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
      const actualResult = testTodoItemsStore.addNew();
      expect(actualResult).toEqual(
        new TodoItemStore(expectedNewTodoItem, givenHttpClient)
      );
      //Assert
      return changePromise;
    });
  });
  describe('Delete todo', () => {
    it('should delete in backend and remove todo from list on success', () => {
      //Arrange
      const givenTodoItems: TodoItem[] = [
        {
          id: 1,
          description: 'given description 1',
          status: 'given status 1',
          dueDate: new Date(),
        },
        {
          id: 2,
          description: 'given description 2',
          status: 'given status 2',
          dueDate: new Date(),
        },
      ];
      const givenMessage = 'given message';
      const givenHttpClient: HttpClient = {} as any;
      testTodoItemsStore = new TodoItemsStore(givenHttpClient);
      testTodoItemsStore.setTodoItems(givenTodoItems, givenMessage);
      const givenTodoItemToDelete = testTodoItemsStore.todoItems[1];

      const expectedTodoItems = testTodoItemsStore.todoItems.filter(
        (_t, index) => index != 1
      );
      const givenDeleteResponse: DeleteTodoItemResponse = {
        message: 'response delete given message',
        validationErrors: {},
      };
      const expectedDeleteTodoItem: DeleteTodoItem = {
        todoItemId: givenTodoItemToDelete.todoItem.id,
      };
      spyHttpClientDelete.and.returnValue(givenDeleteResponse);
      //Act
      testTodoItemsStore.delete(givenTodoItemToDelete);

      //Assert
      expect(spyHttpClientDelete).toHaveBeenCalledWith(
        '/api/todo-items/delete',
        expectedDeleteTodoItem
      );
      expect(testTodoItemsStore.todoItems).toEqual(expectedTodoItems);
      expect(testTodoItemsStore.actionMessage).toEqual('Removing todo item');
    });
    it('should update message when unexpected error', async () => {
      //Arrange
      const givenResponse = {
        type: 'https://tools.ietf.org/html/rfc9110#section-15.6.1',
        title: 'An error occurred while processing your request.',
        status: 500,
        detail: 'Response error',
        traceId: '00-c43d2d9e194869fa4d67a545628fdf8e-7ff541343c3630e0-00',
      };
      spyHttpClientDelete.and.returnValue(
        new Observable((subscriber) => {
          subscriber.error(givenResponse);
          subscriber.complete();
        })
      );

      const expectedChangePromise = new Promise<void>((resolve, reject) => {
        reaction(
          () => ({
            r1: testTodoItemsStore.todoItems,
            r2: testTodoItemsStore.actionMessage,
            r3: testTodoItemsStore.actionValidationErrors,
          }),
          (_arg, _prev, r) => {
            try {
              expect(_arg).toEqual({
                r1: [],
                r2: givenResponse.detail,
                r3: {},
              });
              resolve();
            } catch (error) {
              reject();
            } finally {
              r.dispose();
            }
          }
        );
      });
      const givenDeleteTodoItem: DeleteTodoItem = {
        todoItemId: 1234,
      };
      const givenTodoItemStore: TodoItemStore = {} as any;
      //Act
      testTodoItemsStore.delete(givenTodoItemStore);
      //Assert
      expect(spyHttpClientDelete).toHaveBeenCalledWith(
        '/api/todo-items/create',
        givenDeleteTodoItem
      );
      return expectedChangePromise;
    });
    it('should onlly update message and validation errors when validation errors', async () => {
      //Arrange
      const givenDeleteTodoItemResponse: DeleteTodoItemResponse = {
        message: 'response message',
        validationErrors: {
          description: ['response validation description'],
        },
      };
      spyHttpClientDelete.and.returnValue(of(givenDeleteTodoItemResponse));

      const expectedChangePromise = new Promise<void>((resolve, reject) => {
        reaction(
          () => ({
            r2: testTodoItemsStore.actionMessage,
            r3: testTodoItemsStore.actionValidationErrors,
          }),
          (_arg, _prev, r) => {
            try {
              expect(_arg).toEqual({
                r2: givenDeleteTodoItemResponse.message,
                r3: givenDeleteTodoItemResponse.validationErrors,
              });
              resolve();
            } catch (error) {
              reject();
            } finally {
              r.dispose();
            }
          }
        );
      });
      const givenDeleteTodoItem: DeleteTodoItem = {
        todoItemId: 1234,
      };
      const givenTodoItemStore: TodoItemStore = {} as any;
      //Act
      testTodoItemsStore.delete(givenTodoItemStore);
      //Assert
      expect(spyHttpClientDelete).toHaveBeenCalledWith(
        '/api/todo-items/delete',
        givenDeleteTodoItem
      );
      return expectedChangePromise;
    });
  });
});
