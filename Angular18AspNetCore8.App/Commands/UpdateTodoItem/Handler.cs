﻿using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTodoItems;
using Angular18AspNetCore8.Core.Entities;
using FluentValidation;

namespace Angular18AspNetCore8.App.Commands.UpdateTodoItem;
public class Handler(ITodoItemsRepository todoTasksRepository, IValidator<Command> validator) : ITodoItemsHandler<Command, Response>
{
  public async Task<Response> Execute(Command command)
  {
    var validationResult = validator.Validate(command);

    if (!validationResult.IsValid)
    {
      return new Response
      {
        HasValidationErrors = true,
        Item = command.Item,
        ValidationErrors = validationResult.ToDictionary()
      };
    }

    var infoToUpdate = command.Item;

    var existingTasks = await todoTasksRepository.GetByIds([command.Item.Id]);
    var existingTask = existingTasks.Single();

    existingTask.Description = infoToUpdate.Description;
    existingTask.Duedate = infoToUpdate.DueDate;
    existingTask.Status = TodoTaskStatusNames.Parse[infoToUpdate.Status];

    await todoTasksRepository.SaveChanges();

    return new Response
    {
      HasValidationErrors = false,
      Item = new TodoItemModel
      {
        Description = existingTask.Description,
        DueDate = existingTask.Duedate,
        Id = command.Item.Id,
        Status = (existingTask.Duedate.HasValue && existingTask.Duedate.Value < DateTimeOffset.Now) ? TodoTaskStatusNames.Format[TodoTaskStatus.Overdue] : TodoTaskStatusNames.Format[existingTask.Status]
      }
    };
  }
}
