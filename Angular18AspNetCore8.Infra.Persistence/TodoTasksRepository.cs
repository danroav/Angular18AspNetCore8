using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Angular18AspNetCore8.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Angular18AspNetCore8.Infra.Persistence;

public class TodoTasksRepository(AppDbContext appDbContext) : ITodoTaskRepository
{
  public Task<List<TodoTask>> GetAll()
  {
    return appDbContext.TodoTasks.ToListAsync();
  }

  public Task<TodoTask> AddNew(string description, DateTimeOffset? dueDate, TodoTaskStatus status)
  {
    throw new NotImplementedException();
  }
}
