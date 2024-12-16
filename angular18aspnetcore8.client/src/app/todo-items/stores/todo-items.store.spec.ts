import { HttpClient } from '@angular/common/http';
import { TodoItemsStore } from './todo-items.store';
import { reaction } from 'mobx';
import { GetAllTodoItemsResponse, TodoItem } from '../models/todo-items-models';
import { of } from 'rxjs';
import { TodoItemStore } from './todo-item.store';

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
  describe('Remove todo', () => {
    it('should remove todo from list', () => {
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
      //Act
      testTodoItemsStore.remove(givenTodoItemToDelete);

      //Assert
      expect(testTodoItemsStore.todoItems).toEqual(expectedTodoItems);
      expect(testTodoItemsStore.actionMessage).toEqual('Removing todo item');
    });
  });
});
