import { HttpClient } from '@angular/common/http';
import {
  CreateTodoItem,
  CreateTodoItemResponse,
  TodoItem,
  ValidationErrors,
} from '../models/todo-items-models';
import { TodoItemStore } from './todo-item.store';
import { Observable, of } from 'rxjs';
import { reaction } from 'mobx';

describe('Todo Items Store', () => {
  let testStore: TodoItemStore;
  const httpClientPostSpy = jasmine.createSpy();
  const mockHttpClient: HttpClient = {
    post: httpClientPostSpy,
  } as any;
  beforeEach(() => {
    httpClientPostSpy.and.stub().calls.reset();
  });
  it('When new instance', () => {
    //Arrange
    //Act
    testStore = new TodoItemStore(mockHttpClient);
    //Assert
    expect(testStore.description).toEqual('');
    expect(testStore.dueDate).not.toBeDefined();
    expect(testStore.id).toEqual(0);
    expect(testStore.status).toEqual('');
    expect(testStore.validationErrors).toEqual({});
  });
  describe('Creating Todo Item', () => {
    it('When Success', async () => {
      //Arrange
      const expectedId = 14;
      const expectedDescription = 'some description';
      const expectedStatus = 'some status';
      const expectedDueDate = undefined;
      const expectedValidationErrors: ValidationErrors<TodoItem> = {};
      const expectedMessage = 'some creation message';

      const givenCreateTodoItem: CreateTodoItem = {
        description: 'create description',
        dueDate: undefined,
        status: 'create status',
      };
      const givenCreateTodoItemResult: CreateTodoItemResponse = {
        message: expectedMessage,
        item: {
          description: expectedDescription,
          status: expectedStatus,
          id: expectedId,
          dueDate: expectedDueDate,
        },
        validationErrors: expectedValidationErrors,
      };

      httpClientPostSpy.and.returnValue(of(givenCreateTodoItemResult));

      //Act
      const testTodoItemStore = new TodoItemStore(mockHttpClient);

      const changePromise = new Promise<void>((resolve, reject) => {
        reaction(
          () => testTodoItemStore.id,
          (_arg, _prev, r) => {
            try {
              expect(testTodoItemStore.description).toEqual(
                expectedDescription
              );
              expect(testTodoItemStore.dueDate).toEqual(expectedDueDate);
              expect(testTodoItemStore.id).toEqual(expectedId);
              expect(testTodoItemStore.status).toEqual(expectedStatus);
              expect(testTodoItemStore.validationErrors).toEqual(
                expectedValidationErrors
              );
              expect(testTodoItemStore.message).toEqual(expectedMessage);
              resolve();
            } catch (error) {
              reject();
            } finally {
              r.dispose();
            }
          }
        );
      });

      testTodoItemStore.create(
        givenCreateTodoItem.description,
        givenCreateTodoItem.status,
        givenCreateTodoItem.dueDate
      );

      //Assert
      expect(mockHttpClient.post).toHaveBeenCalledWith(
        '/api/todo-items/create',
        givenCreateTodoItem
      );

      return changePromise;
    });
    it('When unexpected error', async () => {
      //Arrange
      const givenExpectionMessage = 'unexpected exception';
      const responseObservable = new Observable((subscriber) => {
        subscriber.error({ status: 500, detail: givenExpectionMessage });
        subscriber.complete();
      });
      httpClientPostSpy.and.returnValue(responseObservable);

      //Act
      const testTodoItemStore = new TodoItemStore(mockHttpClient);

      const changePromise = new Promise<void>((resolve, reject) => {
        reaction(
          () => testTodoItemStore.message,
          (arg, _prev, r) => {
            try {
              expect(arg).toEqual(givenExpectionMessage);
              expect(testTodoItemStore.validationErrors).toEqual({});
              resolve();
            } catch (error) {
              reject();
            } finally {
              r.dispose();
            }
          }
        );
      });

      testTodoItemStore.create(
        'create description',
        'create status',
        undefined
      );

      //Assert
      return changePromise;
    });
    it('When validation errors', async () => {
      //Arrange
      const givenId = 1;
      const givenDescription = 'some description';
      const givenStatus = 'some status';
      const givenDueDate = undefined;
      const givenValidationErrors: ValidationErrors<TodoItem> = {
        description: ['Some description error'],
      };
      const givenMessage = 'some creation message';

      const givenCreateTodoItem: CreateTodoItem = {
        description: 'create description',
        dueDate: undefined,
        status: 'create status',
      };
      const givenCreateTodoItemResult: CreateTodoItemResponse = {
        message: givenMessage,
        item: {
          description: givenDescription,
          status: givenStatus,
          id: givenId,
          dueDate: givenDueDate,
        },
        validationErrors: givenValidationErrors,
      };

      httpClientPostSpy.and.returnValue(of(givenCreateTodoItemResult));

      //Act
      const testTodoItemStore = new TodoItemStore(mockHttpClient);

      const changePromise = new Promise<void>((resolve, reject) => {
        reaction(
          () => testTodoItemStore.validationErrors,
          (_arg, _prev, r) => {
            try {
              expect(testTodoItemStore.description).toEqual('');
              expect(testTodoItemStore.dueDate).toEqual(undefined);
              expect(testTodoItemStore.id).toEqual(0);
              expect(testTodoItemStore.status).toEqual('');
              expect(testTodoItemStore.validationErrors).toEqual(
                givenValidationErrors
              );
              expect(testTodoItemStore.message).toEqual(givenMessage);
              resolve();
            } catch (error) {
              reject();
            } finally {
              r.dispose();
            }
          }
        );
      });

      testTodoItemStore.create(
        givenCreateTodoItem.description,
        givenCreateTodoItem.status,
        givenCreateTodoItem.dueDate
      );

      //Assert
      return changePromise;
    });
  });
});
