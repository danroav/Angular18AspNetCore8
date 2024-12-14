using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Angular18AspNetCore8.Infra.Persistence;

public class TodoTasksRepository(AppDbContext appDbContext) : ITodoItemsRepository
{
  public Task<List<TodoTask>> GetAll()
  {
    return appDbContext.TodoTasks.ToListAsync();
  }

  public async Task<TodoTask> AddNew(string description, DateTimeOffset? dueDate, TodoTaskStatus status)
  {
    var result = await appDbContext.TodoTasks.AddAsync(new TodoTask { Description = description, Duedate = dueDate, Status = status });
    return result.Entity;
  }

  public Task<List<TodoTask>> GetByIds(IList<int> ids)
  {
    return appDbContext.TodoTasks.Where(x => ids.Contains(x.Id)).ToListAsync();
  }

  public Task<int> SaveChanges()
  {
    return appDbContext.SaveChangesAsync();
  }
}
