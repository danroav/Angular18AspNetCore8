using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;
using Angular18AspNetCore8.Server.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace Angular18AspNetCore8.Server.Tests
{
  public class TodoItemsControllerTests
  {
    readonly TodoItemsController todoItemsController;
    readonly Mock<ITodoItemsHandler<App.Queries.GetAllTodoItems.Query, App.Queries.GetAllTodoItems.Response>> mockGetAllTasks;
    readonly Mock<ITodoItemsHandler<App.Commands.AddNewTodoItem.Command, App.Commands.AddNewTodoItem.Response>> mockAddNewTaskHandler;
    readonly Mock<ITodoItemsHandler<App.Commands.UpdateTodoItem.Command, App.Commands.UpdateTodoItem.Response>> mockUpdateTaskHandler;
    public TodoItemsControllerTests()
    {
      mockGetAllTasks = new Mock<ITodoItemsHandler<App.Queries.GetAllTodoItems.Query, App.Queries.GetAllTodoItems.Response>>();
      mockAddNewTaskHandler = new Mock<ITodoItemsHandler<App.Commands.AddNewTodoItem.Command, App.Commands.AddNewTodoItem.Response>>();
      mockUpdateTaskHandler = new Mock<ITodoItemsHandler<App.Commands.UpdateTodoItem.Command, App.Commands.UpdateTodoItem.Response>>();
      todoItemsController = new TodoItemsController(mockGetAllTasks.Object, mockAddNewTaskHandler.Object, mockUpdateTaskHandler.Object);
    }

    [Fact]
    public async Task GetAllTasksSuccess()
    {
      //Arrange
      var expectedResult = new App.Queries.GetAllTodoItems.Response
      {
        Count = 2,
        Message = "",
        Items = [new TodoItemModel(), new TodoItemModel()]
      };

      mockGetAllTasks.Setup(x => x.Execute(It.IsAny<App.Queries.GetAllTodoItems.Query>())).ReturnsAsync(expectedResult);

      //Act
      var actualResult = await todoItemsController.GetAllTasks();

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Queries.GetAllTodoItems.Response>>();
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
      mockGetAllTasks.Setup(x => x.Execute(It.IsAny<App.Queries.GetAllTodoItems.Query>())).ThrowsAsync(new Exception(expectedExceptionMessage));
      var expectedProblem = new ProblemDetails
      {
        Detail = expectedExceptionMessage,
        Status = expectedStatus
      };

      //Act
      var actualResult = await todoItemsController.GetAllTasks();

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Queries.GetAllTodoItems.Response>>();
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
      var givenCommand = new App.Commands.AddNewTodoItem.Command
      {
        Description = "some description",
        DueDate = null,
        Status = "",
      };
      var expectedResult = new App.Commands.AddNewTodoItem.Response
      {
        Item = new TodoItemModel
        {
          Id = 0,
          Description = givenCommand.Description,
          DueDate = givenCommand.DueDate,
          Status = givenCommand.Status,
        }
      };


      mockAddNewTaskHandler.Setup(x => x.Execute(It.IsAny<App.Commands.AddNewTodoItem.Command>())).ReturnsAsync(expectedResult);

      //Act
      var actualResult = await todoItemsController.AddNewTask(givenCommand);

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Commands.AddNewTodoItem.Response>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<OkObjectResult>();
      ((OkObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
    }
    [Fact]
    public async Task AddNewTaskUnexpectedError()
    {
      //Arrange
      var givenCommand = new App.Commands.AddNewTodoItem.Command
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

      mockAddNewTaskHandler.Setup(x => x.Execute(It.IsAny<App.Commands.AddNewTodoItem.Command>())).ThrowsAsync(new Exception(expectedErrorMessage));

      //Act
      var actualResult = await todoItemsController.AddNewTask(givenCommand);

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Commands.AddNewTodoItem.Response>>();
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
      var givenCommand = new App.Commands.AddNewTodoItem.Command
      {
        Description = "some description",
        DueDate = null,
        Status = "",
      };
      var expectedResult = new App.Commands.AddNewTodoItem.Response
      {
        Item = new TodoItemModel
        {
          Description = givenCommand.Description,
          DueDate = givenCommand.DueDate,
          Id = 0,
          Status = givenCommand.Status,
        },
        HasValidationErrors = true,
        ValidationErrors = new Dictionary<string, string[]>() { { "someProperty", ["someValidationError"] } }
      };


      mockAddNewTaskHandler.Setup(x => x.Execute(It.IsAny<App.Commands.AddNewTodoItem.Command>())).ReturnsAsync(expectedResult);

      //Act
      var actualResult = await todoItemsController.AddNewTask(givenCommand);

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Commands.AddNewTodoItem.Response>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<BadRequestObjectResult>();
      ((BadRequestObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
    }
    [Fact]
    public async Task UpdateTaskSuccess()
    {
      //Arrange
      var expectedResult = new App.Commands.UpdateTodoItem.Response
      {
        Item = new TodoItemModel { Description = "Some Description", DueDate = null, Id = 123, Status = TodoItemStatusNames.Format[TodoItemStatus.ToDo] },
        HasValidationErrors = false,
        ValidationErrors = new Dictionary<string, string[]>()
      };
      var givenCommand = new App.Commands.UpdateTodoItem.Command
      {
        Item = new TodoItemModel { Description = "Some Description", DueDate = null, Id = 123, Status = TodoItemStatusNames.Format[TodoItemStatus.ToDo] }
      };

      mockUpdateTaskHandler.Setup(x => x.Execute(It.IsAny<App.Commands.UpdateTodoItem.Command>())).ReturnsAsync(expectedResult);

      //Act
      var actualResult = await todoItemsController.UpdateTask(givenCommand);

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Commands.UpdateTodoItem.Response>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<OkObjectResult>();
      ((OkObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
    }
    [Fact]
    public async Task UpdateTaskUnexpectedError()
    {
      //Arrange
      var givenCommand = new App.Commands.UpdateTodoItem.Command
      {
        Item = new TodoItemModel { Description = "Some Description", DueDate = null, Id = 123, Status = TodoItemStatusNames.Format[TodoItemStatus.ToDo] }
      };
      var expectedErrorMessage = "Unexpected error";
      var expectedStatus = (int)HttpStatusCode.InternalServerError;
      var expectedProblem = new ProblemDetails
      {
        Detail = expectedErrorMessage,
        Status = expectedStatus
      };

      mockUpdateTaskHandler.Setup(x => x.Execute(It.IsAny<App.Commands.UpdateTodoItem.Command>())).ThrowsAsync(new Exception(expectedErrorMessage));

      //Act
      var actualResult = await todoItemsController.UpdateTask(givenCommand);

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Commands.UpdateTodoItem.Response>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<ObjectResult>();
      var problem = ((ObjectResult)actualResult.Result!);
      problem.StatusCode.Should().Be(expectedStatus);
      problem.Value.Should().BeEquivalentTo(expectedProblem);
    }
    [Fact]
    public async Task UpdateTaskRequestError()
    {
      //Arrange
      var givenItem = new TodoItemModel
      {
        Description = "Some Description",
        DueDate = null,
        Id = 123,
        Status = TodoItemStatusNames.Format[TodoItemStatus.ToDo]
      };
      var expectedResult = new App.Commands.UpdateTodoItem.Response
      {
        Item = givenItem,
        HasValidationErrors = true,
        ValidationErrors = new Dictionary<string, string[]>{
          { "someProperty",["Some validation Error"] }
        }
      };
      var givenCommand = new App.Commands.UpdateTodoItem.Command
      {
        Item = givenItem
      };

      mockUpdateTaskHandler.Setup(x => x.Execute(It.IsAny<App.Commands.UpdateTodoItem.Command>())).ReturnsAsync(expectedResult);

      //Act
      var actualResult = await todoItemsController.UpdateTask(givenCommand);

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Commands.UpdateTodoItem.Response>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<BadRequestObjectResult>();
      ((BadRequestObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
    }
  }
}