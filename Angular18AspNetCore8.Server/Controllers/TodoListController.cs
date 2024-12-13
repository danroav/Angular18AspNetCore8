﻿using Angular18AspNetCore8.App.Commands.AddNewTask;
using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Angular18AspNetCore8.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoListController(IQueryGetAllTasks queryGetlAllTasks, ICommandHandler<CommandAddNewTask, CommandAddNewTaskResult> addNewTaskHandler) : ControllerBase
{
  [HttpGet("get-all")]
  public async Task<ActionResult<QueryGetAllTasksResult>> GetAllTasks()
  {
    try
    {
      return Ok(await queryGetlAllTasks.Execute());
    }
    catch (Exception ex)
    {
      return Problem(detail: ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
    }
  }
  [HttpPost("add")]
  public async Task<ActionResult<CommandAddNewTaskResult>> AddNewTask([FromBody] CommandAddNewTask input)
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
}
