using Testcontainers.PostgreSql;
using TodoApi.Models;
using TodoApi.Repositories;
using Microsoft.Extensions.Configuration;

namespace TodoApi.Tests.Integration.Repositories;

public class TodoRepositoryTest
{
    private PostgreSqlContainer _pgContainer = null!;
    private ApplicationDbContext _dbContext = null!;
    private TodoRepository _repository = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _pgContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await _pgContainer.StartAsync();
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _pgContainer.GetConnectionString()
            })
            .Build();

        _dbContext = new ApplicationDbContext(configuration);

        // Ensure DB schema is created
        await _dbContext.Database.EnsureCreatedAsync();

        _repository = new TodoRepository(_dbContext);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _dbContext.DisposeAsync();
        await _pgContainer.StopAsync();
        await _pgContainer.DisposeAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        // Clean DB before each test
        _dbContext.TodoItems.RemoveRange(_dbContext.TodoItems);
        await _dbContext.SaveChangesAsync();
    }

    [Test]
    public async Task GivenNewTodoItem_WhenCreateAsync_ThenShouldAddToDatabase()
    {
        // Given
        var todo = new TodoItem { Name = "Test task", IsComplete = false };

        // When
        var created = await _repository.CreateAsync(todo);

        // Then
        Assert.That(created.Id, Is.GreaterThan(0));
        Assert.That(created.Name, Is.EqualTo("Test task"));

        var fromDb = await _repository.GetByIdAsync(created.Id);
        Assert.That(fromDb, Is.Not.Null);
    }

    [Test]
    public async Task GivenMultipleTodoItems_WhenGetAllAsync_ThenShouldReturnAllItems()
    {
        // Given
        await _repository.CreateAsync(new TodoItem { Name = "Task 1", IsComplete = false });
        await _repository.CreateAsync(new TodoItem { Name = "Task 2", IsComplete = true });

        // When
        var items = await _repository.GetAllAsync();

        // Then
        Assert.That(items, Has.Exactly(2).Items);
    }

    [Test]
    public async Task GivenExistingTodoItem_WhenUpdateAsync_ThenShouldModifyInDatabase()
    {
        // Given
        var todo = new TodoItem { Name = "Initial", IsComplete = false };
        await _repository.CreateAsync(todo);

        // When
        todo.Name = "Updated";
        todo.IsComplete = true;
        await _repository.UpdateAsync(todo);

        // Then
        var updated = await _repository.GetByIdAsync(todo.Id);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Name, Is.EqualTo("Updated"));
        Assert.That(updated.IsComplete, Is.True);
    }

    [Test]
    public async Task GivenExistingTodoItem_WhenDeleteAsync_ThenShouldRemoveFromDatabase()
    {
        // Given
        var todo = new TodoItem { Name = "To delete", IsComplete = false };
        await _repository.CreateAsync(todo);

        // When
        await _repository.DeleteAsync(todo.Id);

        // Then
        var exists = await _repository.ExistsAsync(todo.Id);
        Assert.That(exists, Is.False);
    }
}
