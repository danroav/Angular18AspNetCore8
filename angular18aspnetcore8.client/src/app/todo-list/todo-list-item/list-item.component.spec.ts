import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ListItemComponent } from './list-item.component';
import { provideHttpClient } from '@angular/common/http';
import { TodoItem } from '../models/todo-list-models';

describe('List Item Component', () => {
  let component: ListItemComponent;
  let fixture: ComponentFixture<ListItemComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ListItemComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ListItemComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create the list item component', async () => {
    expect(component).toBeTruthy();
    const expectedItem: TodoItem = {
      description: 'expected item description',
      id: 4862,
      status: 'expected item status',
      dueDate: 'expected item due date',
    };
    fixture.componentRef.setInput('item', expectedItem);
    fixture.detectChanges();
    const listItemEl = fixture.nativeElement as HTMLElement;
    expect(listItemEl.textContent).toContain(expectedItem.description);
    expect(listItemEl.textContent).toContain(expectedItem.id);
    expect(listItemEl.textContent).toContain(expectedItem.status);
    expect(listItemEl.textContent).toContain(expectedItem.dueDate);
  });
});
