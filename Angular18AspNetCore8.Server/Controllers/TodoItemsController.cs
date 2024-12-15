using Angular18AspNetCore8.App.Common;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Angular18AspNetCore8.Server.Controllers;

[Route("api/todo-items")]
[ApiController]
public class TodoItemsController(ITodoItemsHandler<App.Queries.GetAllTodoItems.GetAllTodoITems, App.Queries.GetAllTodoItems.GetAllTodoItemsResult> getlAllTodoItems, ITodoItemsHandler<App.Commands.AddNewTodoItem.AddNewTodoItem, App.Commands.AddNewTodoItem.AddNewTodoItemResult> addNewTodoItemHandler, ITodoItemsHandler<App.Commands.UpdateTodoItem.UpdateTodoItem, App.Commands.UpdateTodoItem.UpdateTodoItemResult> updateTodoItemHandler) : ControllerBase
{
  [HttpGet("index")]
  public async Task<ActionResult<App.Queries.GetAllTodoItems.GetAllTodoItemsResult>> GetAllTodoItems()
  {
    try
    {
      return Ok(await getlAllTodoItems.Execute(new App.Queries.GetAllTodoItems.GetAllTodoITems()));
    }
    catch (Exception ex)
    {
      return Problem(detail: ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
    }
  }
  [HttpPost("create")]
  public async Task<ActionResult<App.Commands.AddNewTodoItem.AddNewTodoItemResult>> AddNewTodoItem([FromBody] App.Commands.AddNewTodoItem.AddNewTodoItem input)
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
  public async Task<ActionResult<App.Commands.UpdateTodoItem.UpdateTodoItemResult>> UpdateTodoItem([FromBody] App.Commands.UpdateTodoItem.UpdateTodoItem input)
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
