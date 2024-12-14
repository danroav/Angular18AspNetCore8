using Angular18AspNetCore8.App.Commands.UpdateTask;
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Angular18AspNetCore8.Server.Controllers;

[Route("api/todo-list")]
[ApiController]
public class TodoListController(ITodoTasksHandler<QueryGetAllTasks, QueryGetAllTasksResult> queryGetlAllTasks, ITodoTasksHandler<App.Commands.AddNewTodoItem.Command, App.Commands.AddNewTodoItem.Response> addNewTaskHandler, ITodoTasksHandler<CommandUpdateTask, CommandUpdateTaskResult> updateTaskHandler) : ControllerBase
{
  [HttpGet("index")]
  public async Task<ActionResult<QueryGetAllTasksResult>> GetAllTasks()
  {
    try
    {
      return Ok(await queryGetlAllTasks.Execute(new QueryGetAllTasks()));
    }
    catch (Exception ex)
    {
      return Problem(detail: ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
    }
  }
  [HttpPost("create")]
  public async Task<ActionResult<App.Commands.AddNewTodoItem.Response>> AddNewTask([FromBody] App.Commands.AddNewTodoItem.Command input)
  {
    try
    {
      var result = await addNewTaskHandler.Execute(input);
      if (result.HasValidationErrors)
      {
        return BadRequest(result);
      }
      return Ok(result);
    }
    catch (Exception ex)
    {
      return Problem(detail: ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
    }
  }
  [HttpPost("update")]
  public async Task<ActionResult<CommandUpdateTaskResult>> UpdateTask([FromBody] CommandUpdateTask input)
  {
    try
    {
      var result = await updateTaskHandler.Execute(input);
      if (result.HasValidationErrors)
      {
        return BadRequest(result);
      }
      return Ok(result);
    }
    catch (Exception ex)
    {
      return Problem(detail: ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
    }

  }
}
