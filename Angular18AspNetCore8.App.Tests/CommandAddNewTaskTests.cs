using Angular18AspNetCore8.App.Commands.AddNewTask;
using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Angular18AspNetCore8.Core.Entities;
using FluentAssertions;
using Moq;

namespace Angular18AspNetCore8.App.Tests
{
  public class CommandAddNewTaskTests
  {
    readonly Mock<ITodoTaskRepository> mockTodoTaskRepository;
    readonly CommandAddNewTaskHandler testAddNewTaskHandler;
    public CommandAddNewTaskTests()
    {
      mockTodoTaskRepository = new Mock<ITodoTaskRepository>();
      testAddNewTaskHandler = new CommandAddNewTaskHandler(mockTodoTaskRepository.Object);
    }
    [Fact]
    public async Task AddTaskSuccess()
    {
      //Arrange
      var givenTodoTaskStatus = TodoTaskStatus.ToDo;
      var givenCommandAddNewTask = new CommandAddNewTask
      {
        Description = "Some description",
        DueDate = null,
        Status = TodoTaskStatusNames.Format[givenTodoTaskStatus],
      };
      var newTodoTask = new TodoTask { Id = 1000, Description = givenCommandAddNewTask.Description, Duedate = givenCommandAddNewTask.DueDate, Status = givenTodoTaskStatus };
      mockTodoTaskRepository.Setup(x => x.AddNew(It.IsAny<string>(), It.IsAny<DateTimeOffset?>(), It.IsAny<TodoTaskStatus>())).ReturnsAsync(newTodoTask);
      var expectedResult = new CommandAddNewTaskResult
      {
        HasValidationErrors = false,
        TaskId = newTodoTask.Id,
        ValidationErrors = []
      };
      //Act
      var actualResult = await testAddNewTaskHandler.Execute(givenCommandAddNewTask);
      //Assert
      mockTodoTaskRepository.VerifyAll();
      actualResult.Should().BeEquivalentTo(expectedResult);
    }
  }
}
