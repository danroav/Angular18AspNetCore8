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
import { ReactiveFormsModule } from '@angular/forms';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

@NgModule({
  declarations: [AppComponent, TodoItemsComponent, TodoItemComponent],
  imports: [BrowserModule, AppRoutingModule, ReactiveFormsModule],
  providers: [provideHttpClient(withInterceptorsFromDi()), TodoItemsStore, provideAnimationsAsync()],
  bootstrap: [AppComponent],
})
export class AppModule {}
