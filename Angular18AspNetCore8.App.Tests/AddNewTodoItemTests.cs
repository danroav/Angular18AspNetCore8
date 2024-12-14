﻿using Angular18AspNetCore8.App.Commands.AddNewTodoItem;
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;
using FluentAssertions;
using Moq;

namespace Angular18AspNetCore8.App.Tests
{
  public class AddNewTodoItemTests
  {
    readonly Mock<ITodoItemsRepository> mockTodoTaskRepository;
    readonly Handler testAddNewTaskHandler;
    readonly Validator validator = new();
    public AddNewTodoItemTests()
    {
      mockTodoTaskRepository = new Mock<ITodoItemsRepository>();
      testAddNewTaskHandler = new Handler(mockTodoTaskRepository.Object, validator);
    }
    [Fact]
    public async Task AddTaskSuccess()
    {
      //Arrange
      var givenTodoTaskStatus = TodoItemStatus.ToDo;
      var givenCommandAddNewTask = new Command
      {
        Description = "Some description",
        DueDate = null,
        Status = TodoItemStatusNames.Format[givenTodoTaskStatus],
      };
      var newTodoTask = new TodoItem { Id = 1000, Description = givenCommandAddNewTask.Description, DueDate = givenCommandAddNewTask.DueDate, Status = givenTodoTaskStatus };
      mockTodoTaskRepository.Setup(x => x.AddNew(It.IsAny<string>(), It.IsAny<DateTimeOffset?>(), It.IsAny<TodoItemStatus>())).ReturnsAsync(newTodoTask);
      mockTodoTaskRepository.Setup(x => x.SaveChanges());
      var expectedResult = new Response
      {
        HasValidationErrors = false,
        Item = new TodoItemModel
        {
          Description = newTodoTask.Description,
          DueDate = newTodoTask.DueDate,
          Id = newTodoTask.Id,
          Status = (newTodoTask.DueDate.HasValue && newTodoTask.DueDate.Value > DateTimeOffset.Now) ? TodoItemStatusNames.Format[TodoItemStatus.Overdue] : TodoItemStatusNames.Format[newTodoTask.Status]
        }
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
      var givenCommand = new Command()
      {
        Description = "Some description",
        DueDate = DateTimeOffset.Now.AddDays(1),
        Status = TodoItemStatusNames.Format[TodoItemStatus.ToDo]
      };
      var expectedError = "Some error description";
      mockTodoTaskRepository.Setup(x => x.AddNew(It.IsAny<string>(), It.IsAny<DateTimeOffset?>(), It.IsAny<TodoItemStatus>())).ThrowsAsync(new Exception(expectedError));

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
      var givenDueDate = DateTimeOffset.Now.AddDays(givenDueDateDaysAdd);
      var givenCommand = new Command()
      {
        Description = givenDescription,
        DueDate = givenDueDate,
        Status = givenStatus
      };
      var expectedResult = new Response
      {
        HasValidationErrors = true,
        Item = new TodoItemModel
        {
          Description = givenDescription,
          DueDate = givenDueDate,
          Id = 0,
          Status = givenStatus
        },
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
