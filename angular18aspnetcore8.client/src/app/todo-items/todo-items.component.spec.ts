import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TodoItemsComponent } from './todo-items.component';
import { provideHttpClient } from '@angular/common/http';
import { TodoItem, TodoItemsIndexResponse } from './models/todo-items-models';

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

  it('should retrieve todo items from the server', () => {
    const givenTodoItems: TodoItem[] = [
      {
        id: 1,
        description: 'item 1',
        status: 'To do',
        dueDate: 'date 1',
      },
      {
        id: 2,
        description: 'item 2',
        status: 'To do',
        dueDate: 'date 2',
      },
    ];
    const givenResponseMessage = `2 items retrieved`;
    const givenResponse: TodoItemsIndexResponse = {
      count: 2,
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
});
