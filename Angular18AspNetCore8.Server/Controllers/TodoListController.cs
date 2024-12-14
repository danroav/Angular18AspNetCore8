using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Angular18AspNetCore8.Server.Controllers;

[Route("api/todo-list")]
[ApiController]
public class TodoListController(ITodoItemsHandler<QueryGetAllTasks, QueryGetAllTasksResult> queryGetlAllTasks, ITodoItemsHandler<App.Commands.AddNewTodoItem.Command, App.Commands.AddNewTodoItem.Response> addNewTaskHandler, ITodoItemsHandler<App.Commands.UpdateTodoItem.Command, App.Commands.UpdateTodoItem.Response> updateTaskHandler) : ControllerBase
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
  public async Task<ActionResult<App.Commands.UpdateTodoItem.Response>> UpdateTask([FromBody] App.Commands.UpdateTodoItem.Command input)
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
