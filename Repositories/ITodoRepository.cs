using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoRepository
{
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<TodoItem?> GetByIdAsync(long id);
    Task<TodoItem> CreateAsync(TodoItem todoItem);
    Task UpdateAsync(TodoItem todoItem);
    Task DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
}