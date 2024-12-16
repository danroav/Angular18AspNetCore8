import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TodoItemsComponent } from './todo-items.component';
import { provideHttpClient } from '@angular/common/http';
import {
  CreateTodoItem,
  CreateTodoItemResponse,
  GetAllTodoItemsResponse,
  TodoItem,
} from './models/todo-items-models';

describe('Todo Items Component', () => {
  let component: TodoItemsComponent;
  let fixture: ComponentFixture<TodoItemsComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TodoItemsComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TodoItemsComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create the todo items component', () => {
    expect(component).toBeTruthy();
  });

  describe('Get all todo items', () => {
    it('should retrieve todo items from the server', () => {
      const givenTodoItems: TodoItem[] = [
        {
          id: 1,
          description: 'item 1',
          status: 'To do',
          dueDate: new Date(),
        },
        {
          id: 2,
          description: 'item 2',
          status: 'To do',
          dueDate: new Date(),
        },
      ];
      const givenResponseMessage = `2 items retrieved`;
      const givenResponse: GetAllTodoItemsResponse = {
        message: givenResponseMessage,
        items: givenTodoItems,
      };

      component.ngOnInit();

      const req = httpMock.expectOne('/api/todo-items/index');
      expect(req.request.method).toEqual('GET');
      req.flush(givenResponse);

      expect(component.items).toEqual(givenTodoItems);
      expect(component.message).toEqual(givenResponseMessage);
    });
    it('should show error message from server', () => {
      const givenResponseErrorMessage = `error message`;
      const givenResponse = {
        type: 'https://tools.ietf.org/html/rfc9110#section-15.6.1',
        title: 'An error occurred while processing your request.',
        status: 500,
        detail: givenResponseErrorMessage,
        traceId: '00-c43d2d9e194869fa4d67a545628fdf8e-7ff541343c3630e0-00',
      };

      component.ngOnInit();

      const req = httpMock.expectOne('/api/todo-items/index');
      req.flush(givenResponse, {
        status: 500,
        statusText: 'Internal Server Error',
      });

      expect(component.items).toEqual([]);
      expect(component.message).toEqual(givenResponseErrorMessage);
    });
  });
});
