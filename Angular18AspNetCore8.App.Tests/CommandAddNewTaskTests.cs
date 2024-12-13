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
    readonly CommandAddNewTaskValidator validator = new();
    public CommandAddNewTaskTests()
    {
      mockTodoTaskRepository = new Mock<ITodoTaskRepository>();
      testAddNewTaskHandler = new CommandAddNewTaskHandler(mockTodoTaskRepository.Object, validator);
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
        TaskId = newTodoTask.Id
      };
      //Act
      var actualResult = await testAddNewTaskHandler.Execute(givenCommandAddNewTask);
      //Assert
      mockTodoTaskRepository.VerifyAll();
      actualResult.Should().BeEquivalentTo(expectedResult);
    }
    [Fact]
    public async Task AddsNewTaskWithError()
    {
      //Arrange
      var givenCommand = new CommandAddNewTask()
      {
        Description = "Some description",
        DueDate = DateTimeOffset.Now.AddDays(1),
        Status = TodoTaskStatusNames.Format[TodoTaskStatus.ToDo]
      };
      var expectedError = "Some error description";
      mockTodoTaskRepository.Setup(x => x.AddNew(It.IsAny<string>(), It.IsAny<DateTimeOffset?>(), It.IsAny<TodoTaskStatus>())).ThrowsAsync(new Exception(expectedError));

      //Act
      var actualResult = () => testAddNewTaskHandler.Execute(givenCommand);

      //Assert
      await actualResult.Should().ThrowAsync<Exception>().WithMessage(expectedError);
    }
    [Theory]
    [InlineData("", "Description is required", -1, "Due Date should be in the future", "", "Status should be valid")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa123456", "Description should not exceed 255 chars", -1, "Due Date should be in the future", "", "Status should be valid")]
    public async Task AddsNewTaskWithValidationErrors(string givenDescription, string givenDescriptionError, int givenDueDateDaysAdd, string givenDueDateError, string givenStatus, string givenStatusError)
    {
      //Arrange
      var givenCommand = new CommandAddNewTask()
      {
        Description = givenDescription,
        DueDate = DateTimeOffset.Now.AddDays(givenDueDateDaysAdd),
        Status = givenStatus
      };
      var expectedResult = new CommandAddNewTaskResult
      {
        HasValidationErrors = true,
        TaskId = 0,
        ValidationErrors = new Dictionary<string, string[]>
        {
          {"Description",[givenDescriptionError] },
          {"DueDate",[givenDueDateError] },
          {"Status",[givenStatusError] }
        }
      };

      //Act
      var actualResult = await testAddNewTaskHandler.Execute(givenCommand);

      //Assert
      actualResult.Should().BeEquivalentTo(expectedResult);
    }
  }
}
