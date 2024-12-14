using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTodoItems;
using Angular18AspNetCore8.Core.Entities;
using FluentValidation;

namespace Angular18AspNetCore8.App.Commands.AddNewTodoItem
{
  public class Handler(ITodoItemsRepository todoTaskRepository, IValidator<Command> validator) : ITodoItemsHandler<Command, Response>
  {
    public async Task<Response> Execute(Command command)
    {
      var result = validator.Validate(command);
      if (!result.IsValid)
      {
        return new Response
        {
          HasValidationErrors = true,
          Item = new TodoItemModel
          {
            Description = command.Description,
            DueDate = command.DueDate,
            Status = command.Status
          },
          ValidationErrors = result.ToDictionary()
        };
      }
      var repositoryResult = await todoTaskRepository.AddNew(command.Description, command.DueDate, TodoItemStatusNames.Parse[command.Status]);

      await todoTaskRepository.SaveChanges();

      return new Response
      {
        HasValidationErrors = false,
        Item = new TodoItemModel
        {
          Description = repositoryResult.Description,
          DueDate = repositoryResult.DueDate,
          Status = (repositoryResult.DueDate.HasValue && repositoryResult.DueDate.Value > DateTimeOffset.Now) ? TodoItemStatusNames.Format[TodoItemStatus.Overdue] : TodoItemStatusNames.Format[repositoryResult.Status],
          Id = repositoryResult.Id
        },
      };
    }
  }
}
