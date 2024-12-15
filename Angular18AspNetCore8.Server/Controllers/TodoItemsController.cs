using Angular18AspNetCore8.App.Commands.AddNewTodoItem;
using Angular18AspNetCore8.App.Commands.UpdateTodoItem;
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTodoItems;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Angular18AspNetCore8.Server.Controllers;

[Route("api/todo-items")]
[ApiController]
public class TodoItemsController(ITodoItemsHandler<GetAllTodoITems, GetAllTodoItemsResult> getlAllTodoItems, ITodoItemsHandler<App.Commands.AddNewTodoItem.AddNewTodoItem, App.Commands.AddNewTodoItem.AddNewTodoItemResult> addNewTodoItemHandler, ITodoItemsHandler<App.Commands.UpdateTodoItem.UpdateTodoItem, App.Commands.UpdateTodoItem.UpdateTodoItemResult> updateTodoItemHandler) : ControllerBase
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
    public async Task<ActionResult<AddNewTodoItemResult>> AddNewTodoItem([FromBody] AddNewTodoItem input)
    {
        try
        {
            var result = await addNewTodoItemHandler.Execute(input);
            return result.ValidationErrors.Count>0 ? (ActionResult<AddNewTodoItemResult>)BadRequest(result) : (ActionResult<AddNewTodoItemResult>)Ok(result);
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
