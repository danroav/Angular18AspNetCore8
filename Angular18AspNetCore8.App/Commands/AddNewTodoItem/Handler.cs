using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Angular18AspNetCore8.Core.Entities;
using FluentValidation;

namespace Angular18AspNetCore8.App.Commands.AddNewTodoItem
{
  public class Handler(ITodoTasksRepository todoTaskRepository, IValidator<Command> validator) : ITodoTasksHandler<Command, Response>
  {
    public async Task<Response> Execute(Command command)
    {
      var result = validator.Validate(command);
      if (!result.IsValid)
      {
        return new Response
        {
          HasValidationErrors = true,
          Item = new ItemResultModel
          {
            Description = command.Description,
            DueDate = command.DueDate,
            Status = command.Status
          },
          ValidationErrors = result.ToDictionary()
        };
      }
      var repositoryResult = await todoTaskRepository.AddNew(command.Description, command.DueDate, TodoTaskStatusNames.Parse[command.Status]);

      await todoTaskRepository.SaveChanges();

      return new Response
      {
        HasValidationErrors = false,
        Item = new ItemResultModel
        {
          Description = repositoryResult.Description,
          DueDate = repositoryResult.Duedate,
          Status = (repositoryResult.Duedate.HasValue && repositoryResult.Duedate.Value > DateTimeOffset.Now) ? TodoTaskStatusNames.Format[TodoTaskStatus.Overdue] : TodoTaskStatusNames.Format[repositoryResult.Status],
          Id = repositoryResult.Id
        },
      };
    }
  }
}
