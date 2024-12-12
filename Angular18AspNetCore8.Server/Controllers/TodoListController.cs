using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Microsoft.AspNetCore.Mvc;

namespace Angular18AspNetCore8.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoListController(IQueryGetAllTasks queryGetlAllTasks) : ControllerBase
{
  [HttpGet("get-all")]
  public async Task<ActionResult<QueryGetAllTasksResult>> GetAllTasks()
  {
    return Ok(await queryGetlAllTasks.Execute());
  }
}
