import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TodoItemComponent } from './todo-item.component';
import { provideHttpClient } from '@angular/common/http';
import { TodoItem } from '../models/todo-items-models';

describe('List Item Component', () => {
  let component: TodoItemComponent;
  let fixture: ComponentFixture<TodoItemComponent>;
  let httpMock: HttpTestingController;

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
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create the list item component', async () => {
    expect(component).toBeTruthy();
    const expectedTodoItem: TodoItem = {
      description: 'expected item description',
      id: 4862,
      status: 'expected item status',
      dueDate: new Date(),
    };
    fixture.componentRef.setInput('item', expectedTodoItem);
    fixture.detectChanges();
    const listItemEl = fixture.nativeElement as HTMLElement;
    expect(listItemEl.textContent).toContain(expectedTodoItem.description);
    expect(listItemEl.textContent).toContain(expectedTodoItem.id);
    expect(listItemEl.textContent).toContain(expectedTodoItem.status);
    expect(listItemEl.textContent).toContain(expectedTodoItem.dueDate);
  });
});
