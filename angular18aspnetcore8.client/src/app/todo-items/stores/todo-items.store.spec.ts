import { HttpClient } from '@angular/common/http';
import { TodoItemsStore, TodoItemStore } from './todo-items.store';
import { reaction } from 'mobx';
import {
  CreateTodoItem,
  CreateTodoItemResponse,
  GetAllTodoItemsResponse,
  TodoItem,
  UpdateTodoItem,
  UpdateTodoItemResponse,
} from '../models/todo-items-models';
import { of, Observable } from 'rxjs';

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
});
describe('Todo Item store', () => {
  let testTodoItemStore: TodoItemStore;
  const givenTodoItem: TodoItem = {
    id: 0,
    description: 'given todo item description',
    status: 'given todo item status',
    dueDate: new Date(),
  };
  const spyHttpClientPost = jasmine.createSpy('httpClientPost');
  const mockHttpClient: HttpClient = {
    post: spyHttpClientPost,
  } as any;
  beforeEach(() => {
    testTodoItemStore = new TodoItemStore(givenTodoItem, mockHttpClient);
  });
  afterEach(() => {
    givenTodoItem.id = 0;
    givenTodoItem.description = 'given todo item description';
    givenTodoItem.status = 'given todo item status';
    givenTodoItem.dueDate = new Date();
    spyHttpClientPost.calls.reset();
  });
  it('Have default values', () => {
    //Arrange
    //Act
    //Assert
    expect(testTodoItemStore.todoItem).toEqual(givenTodoItem);
    expect(testTodoItemStore.actionMessage).toEqual('');
    expect(testTodoItemStore.actionValidationErrors).toEqual({});
  });
  describe('Save', () => {
    describe('New todo item', () => {
      it('should update state when success', async () => {
        //Arrange
        const givenCreateTodoItemResponse: CreateTodoItemResponse = {
          item: {
            description: 'response description',
            id: 12,
            status: 'response status',
            dueDate: new Date(),
          },
          message: 'response message',
          validationErrors: {},
        };
        spyHttpClientPost.and.returnValue(of(givenCreateTodoItemResponse));

        const expectedChangePromise = new Promise<void>((resolve, reject) => {
          reaction(
            () => ({
              r1: testTodoItemStore.todoItem,
              r2: testTodoItemStore.actionMessage,
              r3: testTodoItemStore.actionValidationErrors,
            }),
            (_arg, _prev, r) => {
              try {
                expect(_arg).toEqual({
                  r1: givenCreateTodoItemResponse.item,
                  r2: givenCreateTodoItemResponse.message,
                  r3: givenCreateTodoItemResponse.validationErrors,
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
        const givenCreateTodoItem: CreateTodoItem = {
          description: 'create description',
          status: 'create status',
          dueDate: new Date(),
        };
        //Act
        testTodoItemStore.save(
          givenCreateTodoItem.description,
          givenCreateTodoItem.status,
          givenCreateTodoItem.dueDate
        );
        //Assert
        expect(spyHttpClientPost).toHaveBeenCalledWith(
          '/api/todo-items/create',
          givenCreateTodoItem
        );
        return expectedChangePromise;
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
        spyHttpClientPost.and.returnValue(
          new Observable((subscriber) => {
            subscriber.error(givenResponse);
            subscriber.complete();
          })
        );

        const expectedChangePromise = new Promise<void>((resolve, reject) => {
          reaction(
            () => ({
              r1: testTodoItemStore.todoItem,
              r2: testTodoItemStore.actionMessage,
              r3: testTodoItemStore.actionValidationErrors,
            }),
            (_arg, _prev, r) => {
              try {
                expect(_arg).toEqual({
                  r1: givenTodoItem,
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
        const givenCreateTodoItem: CreateTodoItem = {
          description: 'create description',
          status: 'create status',
          dueDate: new Date(),
        };
        //Act
        testTodoItemStore.save(
          givenCreateTodoItem.description,
          givenCreateTodoItem.status,
          givenCreateTodoItem.dueDate
        );
        //Assert
        expect(spyHttpClientPost).toHaveBeenCalledWith(
          '/api/todo-items/create',
          givenCreateTodoItem
        );
        return expectedChangePromise;
      });
      it('should onlly update message and validation errors when validation errors', async () => {
        //Arrange
        const givenCreateTodoItemResponse: CreateTodoItemResponse = {
          item: {
            description: 'response description',
            id: 12,
            status: 'response status',
            dueDate: new Date(),
          },
          message: 'response message',
          validationErrors: {
            description: ['response validation description'],
          },
        };
        spyHttpClientPost.and.returnValue(of(givenCreateTodoItemResponse));

        const expectedChangePromise = new Promise<void>((resolve, reject) => {
          reaction(
            () => ({
              r1: testTodoItemStore.todoItem,
              r2: testTodoItemStore.actionMessage,
              r3: testTodoItemStore.actionValidationErrors,
            }),
            (_arg, _prev, r) => {
              try {
                expect(_arg).toEqual({
                  r1: givenTodoItem,
                  r2: givenCreateTodoItemResponse.message,
                  r3: givenCreateTodoItemResponse.validationErrors,
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
        const givenCreateTodoItem: CreateTodoItem = {
          description: 'create description',
          status: 'create status',
          dueDate: new Date(),
        };
        //Act
        testTodoItemStore.save(
          givenCreateTodoItem.description,
          givenCreateTodoItem.status,
          givenCreateTodoItem.dueDate
        );
        //Assert
        expect(spyHttpClientPost).toHaveBeenCalledWith(
          '/api/todo-items/create',
          givenCreateTodoItem
        );
        return expectedChangePromise;
      });
    });
    describe('Existing todo item', () => {
      it('should update state when success', async () => {
        //Arrange
        givenTodoItem.id = 1234;
        const givenUpdateTodoItemResponse: UpdateTodoItemResponse = {
          item: {
            description: 'update response description',
            id: givenTodoItem.id,
            status: 'update response status',
            dueDate: new Date(),
          },
          message: 'update response message',
          validationErrors: {},
        };
        spyHttpClientPost.and.returnValue(of(givenUpdateTodoItemResponse));

        const givenUpdateTodoItem: UpdateTodoItem = {
          item: {
            id: givenTodoItem.id,
            description: 'update description',
            status: 'update status',
            dueDate: new Date(),
          },
        };

        testTodoItemStore = new TodoItemStore(givenTodoItem, mockHttpClient);
        const expectedChangePromise = new Promise<void>((resolve, reject) => {
          reaction(
            () => ({
              r1: testTodoItemStore.todoItem,
              r2: testTodoItemStore.actionMessage,
              r3: testTodoItemStore.actionValidationErrors,
            }),
            (_arg, _prev, r) => {
              try {
                expect(_arg).toEqual({
                  r1: givenUpdateTodoItemResponse.item,
                  r2: givenUpdateTodoItemResponse.message,
                  r3: givenUpdateTodoItemResponse.validationErrors,
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
        testTodoItemStore.save(
          givenUpdateTodoItem.item.description,
          givenUpdateTodoItem.item.status,
          givenUpdateTodoItem.item.dueDate
        );
        //Assert
        expect(spyHttpClientPost).toHaveBeenCalledWith(
          '/api/todo-items/update',
          givenUpdateTodoItem
        );
        return expectedChangePromise;
      });

      it('should update message when unexpected error', async () => {
        //Arrange
        givenTodoItem.id = 1234;
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
        const givenUpdateTodoItem: UpdateTodoItem = {
          item: {
            id: givenTodoItem.id,
            description: 'update description',
            status: 'update status',
            dueDate: new Date(),
          },
        };
        testTodoItemStore = new TodoItemStore(givenTodoItem, mockHttpClient);
        const expectedChangePromise = new Promise<void>((resolve, reject) => {
          reaction(
            () => ({
              r1: testTodoItemStore.todoItem,
              r2: testTodoItemStore.actionMessage,
              r3: testTodoItemStore.actionValidationErrors,
            }),
            (_arg, _prev, r) => {
              try {
                expect(_arg).toEqual({
                  r1: givenTodoItem,
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

        //Act
        testTodoItemStore.save(
          givenUpdateTodoItem.item.description,
          givenUpdateTodoItem.item.status,
          givenUpdateTodoItem.item.dueDate
        );
        //Assert
        expect(spyHttpClientPost).toHaveBeenCalledWith(
          '/api/todo-items/update',
          givenUpdateTodoItem
        );
        return expectedChangePromise;
      });

      it('should onlly update message and validation errors when validation errors', async () => {
        //Arrange
        givenTodoItem.id = 4567;
        const givenUpdateTodoItemResponse: UpdateTodoItemResponse = {
          item: {
            description: 'update response description',
            id: 12,
            status: 'update response status',
            dueDate: new Date(),
          },
          message: 'update response message',
          validationErrors: {
            description: ['update response validation description'],
          },
        };
        spyHttpClientPost.and.returnValue(of(givenUpdateTodoItemResponse));
        const givenUpdateTodoItem: UpdateTodoItem = {
          item: {
            id: givenTodoItem.id,
            description: 'create description',
            status: 'create status',
            dueDate: new Date(),
          },
        };

        testTodoItemStore = new TodoItemStore(givenTodoItem, mockHttpClient);
        const expectedChangePromise = new Promise<void>((resolve, reject) => {
          reaction(
            () => ({
              r1: testTodoItemStore.todoItem,
              r2: testTodoItemStore.actionMessage,
              r3: testTodoItemStore.actionValidationErrors,
            }),
            (_arg, _prev, r) => {
              try {
                expect(_arg).toEqual({
                  r1: givenTodoItem,
                  r2: givenUpdateTodoItemResponse.message,
                  r3: givenUpdateTodoItemResponse.validationErrors,
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
        testTodoItemStore.save(
          givenUpdateTodoItem.item.description,
          givenUpdateTodoItem.item.status,
          givenUpdateTodoItem.item.dueDate
        );
        //Assert
        expect(spyHttpClientPost).toHaveBeenCalledWith(
          '/api/todo-items/update',
          givenUpdateTodoItem
        );
        return expectedChangePromise;
      });
    });
  });
});
