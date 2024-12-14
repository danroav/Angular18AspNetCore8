using Angular18AspNetCore8.Core.Entities;

namespace Angular18AspNetCore8.App.Common
{
  public interface ITodoItemsRepository
  {
    Task<TodoTask> AddNew(string description, DateTimeOffset? dueDate, TodoTaskStatus status);
    Task<List<TodoTask>> GetAll();
    Task<List<TodoTask>> GetByIds(IList<int> ids);
    Task<int> SaveChanges();
  }
}
