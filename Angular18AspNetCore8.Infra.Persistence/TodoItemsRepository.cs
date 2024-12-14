using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Angular18AspNetCore8.Infra.Persistence;

public class TodoItemsRepository(AppDbContext appDbContext) : ITodoItemsRepository
{
  public Task<List<TodoItem>> GetAll()
  {
    return appDbContext.TodoItems.ToListAsync();
  }

  public async Task<TodoItem> AddNew(string description, DateTimeOffset? dueDate, TodoItemStatus status)
  {
    var result = await appDbContext.TodoItems.AddAsync(new TodoItem { Description = description, DueDate = dueDate, Status = status });
    return result.Entity;
  }

  public Task<List<TodoItem>> GetByIds(IList<int> ids)
  {
    return appDbContext.TodoItems.Where(x => ids.Contains(x.Id)).ToListAsync();
  }

  public Task<int> SaveChanges()
  {
    return appDbContext.SaveChangesAsync();
  }
}
