using Angular18AspNetCore8.App.Commands.CreateTodoItem;
using Angular18AspNetCore8.App.Commands.DeleteTodoItem;
using Angular18AspNetCore8.App.Commands.UpdateTodoItem;
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTodoItems;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Angular18AspNetCore8.Server.Controllers;

[Route("api/todo-items")]
[ApiController]
public class TodoItemsController(ITodoItemsHandler<GetAllTodoITems, GetAllTodoItemsResult> getlAllTodoItems, ITodoItemsHandler<CreateTodoItem, CreateTodoItemResult> createTodoItemHandler, ITodoItemsHandler<UpdateTodoItem, UpdateTodoItemResult> updateTodoItemHandler, ITodoItemsHandler<DeleteTodoItem, DeleteTodoItemResult> deleteTodoItemHandler) : ControllerBase
{
  [HttpGet("index")]
  public async Task<ActionResult<GetAllTodoItemsResult>> GetAllTodoItems()
  {
    try
    {
      return Ok(await getlAllTodoItems.Execute(new GetAllTodoITems()));
    }
    catch (Exception ex)
    {
      return Problem(detail: ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
    }
  }
  [HttpPost("create")]
  public async Task<ActionResult<CreateTodoItemResult>> CreateTodoItem([FromBody] CreateTodoItem input)
  {
    try
    {
      var result = await createTodoItemHandler.Execute(input);
      return result.ValidationErrors.Count > 0 ? (ActionResult<CreateTodoItemResult>)BadRequest(result) : (ActionResult<CreateTodoItemResult>)Ok(result);
    }
    catch (Exception ex)
    {
      return Problem(detail: ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
    }
  }
  [HttpPost("update")]
  public async Task<ActionResult<UpdateTodoItemResult>> UpdateTodoItem([FromBody] UpdateTodoItem input)
  {
    try
    {
      var result = await updateTodoItemHandler.Execute(input);
      if (result.ValidationErrors.Count > 0)
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
  [HttpDelete("delete")]
  public async Task<ActionResult<DeleteTodoItemResult>> DeleteTodoItem([FromBody] DeleteTodoItem input)
  {
    try
    {
      var result = await deleteTodoItemHandler.Execute(input);
      if (result.ValidationErrors.Count > 0)
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
