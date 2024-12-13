using Angular18AspNetCore8.App.Commands.UpdateTask;
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Angular18AspNetCore8.Core.Entities;
using System.Globalization;

namespace Angular18AspNetCore8.App.Tests;

public class CommandUpdateTaskHandler : ITodoTasksHandler<CommandUpdateTask, CommandUpdateTaskResult>
{
  private ITodoTasksRepository todoTasksRepository;

  public CommandUpdateTaskHandler(ITodoTasksRepository todoTasksRepository)
  {
    this.todoTasksRepository = todoTasksRepository;
  }

  public async Task<CommandUpdateTaskResult> Execute(CommandUpdateTask command)
  {
    var infoToUpdate = command.Item;

    var existingTasks = await todoTasksRepository.GetByIds([command.Item.Id]);
    var existingTask = existingTasks.Single();

    existingTask.Description = infoToUpdate.Description;
    existingTask.Duedate = string.IsNullOrEmpty(infoToUpdate.DueDate) ? null : DateTimeOffset.ParseExact(infoToUpdate.DueDate, "D", CultureInfo.InvariantCulture);
    existingTask.Status = TodoTaskStatusNames.Parse[infoToUpdate.Status];

    await todoTasksRepository.SaveChanges();

    return new CommandUpdateTaskResult
    {
      HasValidationErrors = false,
      Item = new ItemResultModel
      {
        Description = existingTask.Description,
        DueDate = existingTask.Duedate.HasValue ? $"{existingTask.Duedate:D}" : "",
        Id = command.Item.Id,
        Status = TodoTaskStatusNames.Format[existingTask.Status]
      }
    };
  }
}