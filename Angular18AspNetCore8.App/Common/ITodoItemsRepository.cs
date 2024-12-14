using Angular18AspNetCore8.Core.Entities;

namespace Angular18AspNetCore8.App.Common
{
  public interface ITodoItemsRepository
  {
    Task<TodoItem> AddNew(string description, DateTimeOffset? dueDate, TodoItemStatus status);
    Task<List<TodoItem>> GetAll();
    Task<List<TodoItem>> GetByIds(IList<int> ids);
    Task<int> SaveChanges();
  }
}
