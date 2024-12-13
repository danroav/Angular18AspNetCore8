using Angular18AspNetCore8.App.Commands.AddNewTask;
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Angular18AspNetCore8.Server.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace Angular18AspNetCore8.Server.Tests
{
  public class TodoListTests
  {
    readonly TodoListController todoListController;
    readonly Mock<IHandler<QueryGetAllTasks, QueryGetAllTasksResult>> mockGetAllTasks;
    readonly Mock<IHandler<CommandAddNewTask, CommandAddNewTaskResult>> mockAddNewTaskHandler;
    public TodoListTests()
    {
      mockGetAllTasks = new Mock<IHandler<QueryGetAllTasks, QueryGetAllTasksResult>>();
      mockAddNewTaskHandler = new Mock<IHandler<CommandAddNewTask, CommandAddNewTaskResult>>();
      todoListController = new TodoListController(mockGetAllTasks.Object, mockAddNewTaskHandler.Object);
    }

    [Fact]
    public async Task GetAllTasksSuccess()
    {
      //Arrange
      var expectedResult = new QueryGetAllTasksResult
      {
        Count = 2,
        Message = "",
        Items = [new ItemResultModel(), new ItemResultModel()]
      };

      mockGetAllTasks.Setup(x => x.Execute(It.IsAny<QueryGetAllTasks>())).ReturnsAsync(expectedResult);

      //Act
      var actualResult = await todoListController.GetAllTasks();

      //Assert
      actualResult.Should().BeOfType<ActionResult<QueryGetAllTasksResult>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<OkObjectResult>();
      ((OkObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
    }
    [Fact]
    public async Task GetAllTasksUnexpectedError()
    {
      //Arrange
      var expectedExceptionMessage = "Some error";
      var expectedStatus = (int)HttpStatusCode.InternalServerError;
      mockGetAllTasks.Setup(x => x.Execute(It.IsAny<QueryGetAllTasks>())).ThrowsAsync(new Exception(expectedExceptionMessage));
      var expectedProblem = new ProblemDetails
      {
        Detail = expectedExceptionMessage,
        Status = expectedStatus
      };

      //Act
      var actualResult = await todoListController.GetAllTasks();

      //Assert
      actualResult.Should().BeOfType<ActionResult<QueryGetAllTasksResult>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<ObjectResult>();
      var problem = ((ObjectResult)actualResult.Result!);
      problem.StatusCode.Should().Be(expectedStatus);
      problem.Value.Should().BeEquivalentTo(expectedProblem);
    }
    [Fact]
    public async Task AddNewTaskSuccess()
    {
      //Arrange
      var expectedResult = new CommandAddNewTaskResult
      {
        TaskId = 3
      };
      var givenCommand = new CommandAddNewTask
      {
        Description = "some description",
        DueDate = null,
        Status = "",
      };

      mockAddNewTaskHandler.Setup(x => x.Execute(It.IsAny<CommandAddNewTask>())).ReturnsAsync(expectedResult);

      //Act
      var actualResult = await todoListController.AddNewTask(givenCommand);

      //Assert
      actualResult.Should().BeOfType<ActionResult<CommandAddNewTaskResult>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<OkObjectResult>();
      ((OkObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
    }
    [Fact]
    public async Task AddNewTaskUnexpectedError()
    {
      //Arrange
      var givenCommand = new CommandAddNewTask
      {
        Description = "some description",
        DueDate = null,
        Status = "",
      };
      var expectedErrorMessage = "Unexpected error";
      var expectedStatus = (int)HttpStatusCode.InternalServerError;
      var expectedProblem = new ProblemDetails
      {
        Detail = expectedErrorMessage,
        Status = expectedStatus
      };

      mockAddNewTaskHandler.Setup(x => x.Execute(It.IsAny<CommandAddNewTask>())).ThrowsAsync(new Exception(expectedErrorMessage));

      //Act
      var actualResult = await todoListController.AddNewTask(givenCommand);

      //Assert
      actualResult.Should().BeOfType<ActionResult<CommandAddNewTaskResult>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<ObjectResult>();
      var problem = ((ObjectResult)actualResult.Result!);
      problem.StatusCode.Should().Be(expectedStatus);
      problem.Value.Should().BeEquivalentTo(expectedProblem);
    }
    [Fact]
    public async Task AddNewTaskRequestError()
    {
      //Arrange
      var expectedResult = new CommandAddNewTaskResult
      {
        TaskId = 0,
        HasValidationErrors = true,
        ValidationErrors = new Dictionary<string, string[]>() { { "someProperty", ["someValidationError"] } }
      };
      var givenCommand = new CommandAddNewTask
      {
        Description = "some description",
        DueDate = null,
        Status = "",
      };

      mockAddNewTaskHandler.Setup(x => x.Execute(It.IsAny<CommandAddNewTask>())).ReturnsAsync(expectedResult);

      //Act
      var actualResult = await todoListController.AddNewTask(givenCommand);

      //Assert
      actualResult.Should().BeOfType<ActionResult<CommandAddNewTaskResult>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<BadRequestObjectResult>();
      ((BadRequestObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
    }
  }
}