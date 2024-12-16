import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient } from '@angular/common/http';
import { TodoItemComponent } from './todo-item.component';
import { TodoItemStore } from '../stores/todo-items.store';

describe('List Item Component', () => {
  let component: TodoItemComponent;
  let fixture: ComponentFixture<TodoItemComponent>;
  let httpMock: HttpTestingController;
  let httpClient: HttpClient;
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TodoItemComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TodoItemComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    httpClient = TestBed.inject(HttpClient);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create the list item component', async () => {
    expect(component).toBeTruthy();
    const givenTodoItem = {
      description: 'expected item description',
      id: 4862,
      status: 'expected item status',
      dueDate: new Date(),
    };
    const expectedTodoItemStore: TodoItemStore = new TodoItemStore(
      givenTodoItem,
      httpClient
    );
    fixture.componentRef.setInput('todoItemStore', expectedTodoItemStore);
    fixture.detectChanges();
    const listItemEl = fixture.nativeElement as HTMLElement;

    expect(listItemEl.textContent).toContain(givenTodoItem.description);
    expect(listItemEl.textContent).toContain(givenTodoItem.id);
    expect(listItemEl.textContent).toContain(givenTodoItem.status);
    expect(listItemEl.textContent).toContain(givenTodoItem.dueDate);
  });
});
