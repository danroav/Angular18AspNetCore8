import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ListComponent } from './list.component';
import { provideHttpClient } from '@angular/common/http';
import { TodoItem, TodoListIndexResponse } from './models/todo-list-models';

describe('List Component', () => {
  let component: ListComponent;
  let fixture: ComponentFixture<ListComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ListComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ListComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create the list component', () => {
    expect(component).toBeTruthy();
  });

  it('should retrieve todo list from the server', () => {
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
    const givenResponse: TodoListIndexResponse = {
      count: 2,
      message: givenResponseMessage,
      items: givenTodoItems,
    };

    component.ngOnInit();

    const req = httpMock.expectOne('/api/todo-list/index');
    expect(req.request.method).toEqual('GET');
    req.flush(givenResponse);

    expect(component.items).toEqual(givenTodoItems);
    expect(component.message).toEqual(givenResponseMessage);
  });
});
