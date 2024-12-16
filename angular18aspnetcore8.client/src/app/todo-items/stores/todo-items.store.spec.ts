import { HttpClient } from '@angular/common/http';
import { TodoItemsStore, TodoItemStore } from './todo-items.store';
import { reaction } from 'mobx';
import {
  CreateTodoItem,
  CreateTodoItemResponse,
  GetAllTodoItemsResponse,
  TodoItem,
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
  const givenInitTodoItem: TodoItem = {
    id: 0,
    description: 'given todo item',
    status: 'given status',
    dueDate: new Date(),
  };
  const spyHttpClientPost = jasmine.createSpy('httpClientPost');
  const mockHttpClient: HttpClient = {
    post: spyHttpClientPost,
  } as any;
  beforeEach(() => {
    testTodoItemStore = new TodoItemStore(givenInitTodoItem, mockHttpClient);
  });
  it('Have default values', () => {
    //Arrange
    //Act
    //Assert
    expect(testTodoItemStore.todoItem).toEqual(givenInitTodoItem);
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
                  r1: givenInitTodoItem,
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
                  r1: givenInitTodoItem,
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
  });
});
