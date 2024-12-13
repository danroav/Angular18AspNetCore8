using Angular18AspNetCore8.Core.Entities;

namespace Angular18AspNetCore8.App.Queries.GetAllTasks
{
  public interface ITodoTaskRepository
  {
    Task<List<TodoTask>> GetAll();
  }
}
