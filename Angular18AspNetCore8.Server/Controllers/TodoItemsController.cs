using Angular18AspNetCore8.App.Common;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Angular18AspNetCore8.Server.Controllers;

[Route("api/todo-list")]
[ApiController]
public class TodoItemsController(ITodoItemsHandler<App.Queries.GetAllTodoItems.Query, App.Queries.GetAllTodoItems.Response> getlAllTodoItems, ITodoItemsHandler<App.Commands.AddNewTodoItem.Command, App.Commands.AddNewTodoItem.Response> addNewTodoItemHandler, ITodoItemsHandler<App.Commands.UpdateTodoItem.Command, App.Commands.UpdateTodoItem.Response> updateTodoItemHandler) : ControllerBase
{
  [HttpGet("index")]
  public async Task<ActionResult<App.Queries.GetAllTodoItems.Response>> GetAllTodoItems()
  {
    try
    {
      return Ok(await getlAllTodoItems.Execute(new App.Queries.GetAllTodoItems.Query()));
    }
    catch (Exception ex)
    {
      return Problem(detail: ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
    }
  }
  [HttpPost("create")]
  public async Task<ActionResult<App.Commands.AddNewTodoItem.Response>> AddNewTodoItem([FromBody] App.Commands.AddNewTodoItem.Command input)
  {
    try
    {
      var result = await addNewTodoItemHandler.Execute(input);
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
  public async Task<ActionResult<App.Commands.UpdateTodoItem.Response>> UpdateTodoItem([FromBody] App.Commands.UpdateTodoItem.Command input)
  {
    try
    {
      var result = await updateTodoItemHandler.Execute(input);
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
