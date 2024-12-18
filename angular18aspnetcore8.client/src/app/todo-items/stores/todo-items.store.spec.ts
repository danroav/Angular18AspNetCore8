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
  const spyHttpClientPost = jasmine.createSpy('httpClientPost');
  const mockHttpClient: HttpClient = {
    get: spyHttpClientGet,
    delete: spyHttpClientPost,
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
                  (t) =>
                    new TodoItemStore(t, testTodoItemsStore, mockHttpClient)
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
      const givenLighterHttpClient: HttpClient = {} as any;
      testTodoItemsStore = new TodoItemsStore(givenLighterHttpClient);
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
                    (t) =>
                      new TodoItemStore(
                        t,
                        testTodoItemsStore,
                        givenLighterHttpClient
                      )
                  ),
                  new TodoItemStore(
                    expectedNewTodoItem,
                    testTodoItemsStore,
                    givenLighterHttpClient
                  ),
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
        new TodoItemStore(
          expectedNewTodoItem,
          testTodoItemsStore,
          givenLighterHttpClient
        )
      );
      //Assert
      return changePromise;
    });
  });
  describe('Delete todo', () => {
    it('should delete in backend and remove todo from list on success', async () => {
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

      const givenLighterHttpClient: HttpClient = {
        post: spyHttpClientPost,
      } as any;

      testTodoItemsStore = new TodoItemsStore(givenLighterHttpClient);
      testTodoItemsStore.setTodoItems(givenTodoItems, givenMessage);

      const givenTodoItemToDelete = testTodoItemsStore.todoItems[1];

      const expectedTodoItems = testTodoItemsStore.todoItems.filter(
        (t) => t !== givenTodoItemToDelete
      );
      const givenResponse: DeleteTodoItemResponse = {
        item: {
          id: givenTodoItemToDelete.todoItem.id,
          description: givenTodoItemToDelete.todoItem.description,
          status: givenTodoItemToDelete.todoItem.status,
          dueDate: givenTodoItemToDelete.todoItem.dueDate,
        },
        message: 'response delete given message',
        validationErrors: {},
      };

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
                r1: expectedTodoItems,
                r2: givenResponse.message,
                r3: givenResponse.validationErrors,
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
      spyHttpClientPost.and.returnValue(of(givenResponse));

      //Act
      testTodoItemsStore.delete(givenTodoItemToDelete);

      //Assert
      expect(spyHttpClientPost).toHaveBeenCalledWith('/api/todo-items/delete', {
        todoItemId: givenTodoItemToDelete.todoItem.id,
      } as DeleteTodoItem);
      return expectedChangePromise;
    });

    it('should update message when unexpected error', async () => {
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

      const givenLighterHttpClient: HttpClient = {
        post: spyHttpClientPost,
      } as any;

      testTodoItemsStore = new TodoItemsStore(givenLighterHttpClient);
      testTodoItemsStore.setTodoItems(givenTodoItems, givenMessage);

      const givenTodoItemStoreToDelete = testTodoItemsStore.todoItems[1];

      const givenResponse = {
        type: 'https://tools.ietf.org/html/rfc9110#section-15.6.1',
        title: 'An error occurred while processing your request.',
        status: 500,
        detail: 'Response error',
        traceId: '00-c43d2d9e194869fa4d67a545628fdf8e-7ff541343c3630e0-00',
      };
      spyHttpClientPost.and.returnValue(
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
                r1: testTodoItemsStore.todoItems,
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
        todoItemId: givenTodoItemStoreToDelete.todoItem.id,
      };
      //Act
      testTodoItemsStore.delete(givenTodoItemStoreToDelete);

      //Assert
      expect(spyHttpClientPost).toHaveBeenCalledWith(
        '/api/todo-items/delete',
        givenDeleteTodoItem
      );
      return expectedChangePromise;
    });

    it('should only update message and validation errors when validation errors', async () => {
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
      const givenLighterHttpClient: HttpClient = {
        post: spyHttpClientPost,
      } as any;
      testTodoItemsStore = new TodoItemsStore(givenLighterHttpClient);
      testTodoItemsStore.setTodoItems(givenTodoItems, givenMessage);

      const givenTodoItemStoreToDelete = testTodoItemsStore.todoItems[1];

      const givenDeleteTodoItemResponse: DeleteTodoItemResponse = {
        message: 'response message',
        validationErrors: {
          description: ['response validation description'],
        },
      };
      spyHttpClientPost.and.returnValue(of(givenDeleteTodoItemResponse));

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
        todoItemId: givenTodoItemStoreToDelete.todoItem.id,
      };
      //Act
      testTodoItemsStore.delete(givenTodoItemStoreToDelete);
      //Assert
      expect(spyHttpClientPost).toHaveBeenCalledWith(
        '/api/todo-items/delete',
        givenDeleteTodoItem
      );
      return expectedChangePromise;
    });
  });
});
