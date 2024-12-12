namespace Angular18AspNetCore8.App.Queries.GetAllTasks;

public interface IQueryGetAllTasks
{
  Task<QueryGetAllTasksResult> Execute();
}

