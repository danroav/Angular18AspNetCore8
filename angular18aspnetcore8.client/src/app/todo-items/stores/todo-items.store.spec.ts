import { HttpClient } from '@angular/common/http';
import {
  AddnewTodoItem,
  AddNewTodoItemResult,
} from '../models/todo-items-models';
import { StoreMode, TodoItemStore } from './todo-items.store';
import { of } from 'rxjs';
import { autorun, reaction } from 'mobx';

describe('Todo Items Store', () => {
  let testStore: TodoItemStore;
  const httpClientPostSpy = jasmine.createSpy();
  const mockHttpClient: HttpClient = {
    post: httpClientPostSpy,
  } as any;
  beforeEach(() => {
    httpClientPostSpy.and.stub().calls.reset();
  });
  it('When creating new instance', () => {
    //Arrange
    //Act
    testStore = new TodoItemStore(mockHttpClient);
    //Assert
    expect(testStore.description).toEqual('');
    expect(testStore.dueDate).not.toBeDefined();
    expect(testStore.id).toEqual(0);
    expect(testStore.mode).toEqual(StoreMode.view);
    expect(testStore.status).toEqual('');
    expect(testStore.validationErrors).toEqual({});
  });

  it('When adding new item success', async () => {
    //Arrange
    const givenResultId = 14;
    const givenResultDescription = 'some description';
    const givenResultStatus = 'some status';
    const givenResultDueDate = undefined;
    const givenResultValidationErrors = {};

    const givenAddNewTodoItem: AddnewTodoItem = {
      description: 'create description',
      dueDate: undefined,
      status: 'create status',
    };
    const givenAddNewTodoItemResult: AddNewTodoItemResult = {
      hasValidationErrors: false,
      item: {
        description: givenResultDescription,
        status: givenResultStatus,
        id: givenResultId,
        dueDate: givenResultDueDate,
      },
      validationErrors: givenResultValidationErrors,
    };

    httpClientPostSpy.and.returnValue(of(givenAddNewTodoItemResult));

    //Act
    const testTodoItemStore = new TodoItemStore(mockHttpClient);

    const changePromise = new Promise<void>((resolve, reject) => {
      reaction(
        () => testTodoItemStore.id,
        (_arg, _prev, r) => {
          try {
            expect(testTodoItemStore.description).toEqual(
              givenResultDescription
            );
            expect(testTodoItemStore.dueDate).toEqual(givenResultDueDate);
            expect(testTodoItemStore.id).toEqual(givenResultId);
            expect(testTodoItemStore.status).toEqual(givenResultStatus);
            expect(testTodoItemStore.validationErrors).toEqual(
              givenResultValidationErrors
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

    testTodoItemStore.create(
      givenAddNewTodoItem.description,
      givenAddNewTodoItem.status,
      givenAddNewTodoItem.dueDate
    );

    //Assert
    expect(mockHttpClient.post).toHaveBeenCalledWith(
      '/api/todo-items/create',
      givenAddNewTodoItem
    );

    return changePromise;
  });
});
