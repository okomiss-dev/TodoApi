using System;
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
        public async Task GetTodoItems_WhenItemsExist_ReturnsOkWithMappedItems()
        {
            // Given
            var items = new List<TodoItem> { new TodoItem { Id = 1, Name = "Test", IsComplete = true } };
            var dtos = new List<TodoItemDTO> { new TodoItemDTO { Id = 1, Name = "Test", IsComplete = true } };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(items);
            _mapperMock.Setup(m => m.Map<IEnumerable<TodoItemDTO>>(items)).Returns(dtos);

            // When
            var result = await _controller.GetTodoItems();

            // Then
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.StatusCode ?? 200, Is.EqualTo(200));
            Assert.That(ok.Value, Is.EqualTo(dtos));
        }

        [Test]
        public async Task GetTodoItem_WhenFound_ReturnsMappedItem()
        {
            // Given
            var entity = new TodoItem { Id = 1, Name = "Test", IsComplete = false };
            var dto = new TodoItemDTO { Id = 1, Name = "Test", IsComplete = false };

            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<TodoItemDTO>(entity)).Returns(dto);

            // When
            var result = await _controller.GetTodoItem(1);

            // Then
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.StatusCode ?? 200, Is.EqualTo(200));
            Assert.That(ok.Value, Is.EqualTo(dto));
        }

        [Test]
        public async Task GetTodoItem_WhenNotFound_ReturnsNotFound()
        {
            // Given
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((TodoItem)null);

            // When
            var result = await _controller.GetTodoItem(1);

            // Then
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task PutTodoItem_WhenIdMismatch_ReturnsBadRequest()
        {
            // Given
            var dto = new TodoItemDTO { Id = 2, Name = "X", IsComplete = false };

            // When
            var result = await _controller.PutTodoItem(1, dto);

            // Then
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PutTodoItem_WhenItemNotFound_ReturnsNotFound()
        {
            // Given
            var dto = new TodoItemDTO { Id = 1, Name = "X", IsComplete = false };
            _mapperMock.Setup(m => m.Map<TodoItem>(dto)).Returns(new TodoItem { Id = 1 });
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>())).ThrowsAsync(new InvalidOperationException("not found"));

            // When
            var result = await _controller.PutTodoItem(1, dto);

            // Then
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task PutTodoItem_WhenConcurrencyIssue_ReturnsConflict()
        {
            // Given
            var dto = new TodoItemDTO { Id = 1, Name = "X", IsComplete = false };
            _mapperMock.Setup(m => m.Map<TodoItem>(dto)).Returns(new TodoItem { Id = 1 });
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>())).ThrowsAsync(new InvalidOperationException("modified by another user"));

            // When
            var result = await _controller.PutTodoItem(1, dto);

            // Then
            Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public async Task PutTodoItem_WhenValid_ReturnsNoContent()
        {
            // Given
            var dto = new TodoItemDTO { Id = 1, Name = "X", IsComplete = false };
            _mapperMock.Setup(m => m.Map<TodoItem>(dto)).Returns(new TodoItem { Id = 1 });

            // When
            var result = await _controller.PutTodoItem(1, dto);

            // Then
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task PostTodoItem_WhenValid_ReturnsCreatedItem()
        {
            // Given
            var dto = new TodoItemDTO { Id = 0, Name = "New", IsComplete = false };
            var entity = new TodoItem { Id = 42, Name = "New", IsComplete = false };
            var resultDto = new TodoItemDTO { Id = 42, Name = "New", IsComplete = false };

            _mapperMock.Setup(m => m.Map<TodoItem>(dto)).Returns(entity);
            _repositoryMock.Setup(r => r.CreateAsync(entity)).ReturnsAsync(entity); 
            _mapperMock.Setup(m => m.Map<TodoItemDTO>(entity)).Returns(resultDto);

            // When
            var result = await _controller.PostTodoItem(dto);

            // Then
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
            var created = result.Result as CreatedAtActionResult;
            Assert.That(created!.StatusCode, Is.EqualTo(201));
            Assert.That(created.Value, Is.EqualTo(resultDto));
        }

        [Test]
        public async Task PostTodoItem_WhenFails_Returns500()
        {
            // Given
            var dto = new TodoItemDTO { Id = 0, Name = "X", IsComplete = false };
            _mapperMock.Setup(m => m.Map<TodoItem>(dto)).Returns(new TodoItem());
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<TodoItem>())).ThrowsAsync(new InvalidOperationException());

            // When
            var result = await _controller.PostTodoItem(dto);

            // Then
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var err = result.Result as ObjectResult;
            Assert.That(err!.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task DeleteTodoItem_WhenNotExists_ReturnsNotFound()
        {
            // Given
            _repositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(false);

            // When
            var result = await _controller.DeleteTodoItem(1);

            // Then
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteTodoItem_WhenExists_ReturnsNoContent()
        {
            // Given
            _repositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

            // When
            var result = await _controller.DeleteTodoItem(1);

            // Then
            _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }
}