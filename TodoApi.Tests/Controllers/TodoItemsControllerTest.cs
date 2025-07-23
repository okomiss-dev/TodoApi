using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Tests.Controllers;

[TestFixture]
[TestOf(typeof(TodoItemsController))]
public class TodoItemsControllerTest
{
    private Mock<ITodoRepository> _repositoryMock;
    private Mock<IMapper> _mapperMock;
    private TodoItemsController _controller;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _mapperMock = new Mock<IMapper>();
        _controller = new TodoItemsController(_repositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GetTodoItems_ReturnsOkResult_WithMappedTodoItems()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = 1, Name = "Test 1", IsComplete = true },
            new TodoItem { Id = 2, Name = "Test 2", IsComplete = false }
        };
        var todoItemDTOs = new List<TodoItemDTO>
        {
            new TodoItemDTO { Id = 1, Name = "Test 1", IsComplete = true },
            new TodoItemDTO { Id = 2, Name = "Test 2", IsComplete = false }
        };

        _repositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(todoItems);
        _mapperMock.Setup(m => m.Map<IEnumerable<TodoItemDTO>>(todoItems)).Returns(todoItemDTOs);

        // Act
        var result = await _controller.GetTodoItems();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.AreEqual(todoItemDTOs, okResult.Value);
    }
}