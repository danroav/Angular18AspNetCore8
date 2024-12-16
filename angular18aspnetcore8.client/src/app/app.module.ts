import {
  provideHttpClient,
  withInterceptorsFromDi,
} from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { TodoItemsStore } from './todo-items/stores/todo-items.store';
import { TodoItemsComponent } from './todo-items/todo-items.component';
import { TodoItemComponent } from './todo-items/todo-item/todo-item.component';

@NgModule({
  declarations: [AppComponent, TodoItemsComponent, TodoItemComponent],
  imports: [BrowserModule, AppRoutingModule],
  providers: [provideHttpClient(withInterceptorsFromDi()), TodoItemsStore],
  bootstrap: [AppComponent],
})
export class AppModule {}
