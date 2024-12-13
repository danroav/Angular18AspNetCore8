using Angular18AspNetCore8.Core.Entities;

namespace Angular18AspNetCore8.App.Common
{
  public interface ITodoTaskRepository
  {
    Task<TodoTask> AddNew(string description, DateTimeOffset? dueDate, TodoTaskStatus status);
    Task<List<TodoTask>> GetAll();
  }
}
