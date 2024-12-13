using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Angular18AspNetCore8.Infra.Persistence;

public class TodoTasksRepository(AppDbContext appDbContext) : ITodoTasksRepository
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
}
