using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly ApplicationDbContext _context;

    public TodoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        return await _context.TodoItems.AsNoTracking().ToListAsync();
    }

    public async Task<TodoItem?> GetByIdAsync(long id)
    {
        return await _context.TodoItems.FindAsync(id);
    }

    public async Task<TodoItem> CreateAsync(TodoItem todoItem)
    {
        try
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            return todoItem;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Failed to create TodoItem", ex);
        }
    }

    public async Task UpdateAsync(TodoItem todoItem)
    {
        var existingItem = await _context.TodoItems.FindAsync(todoItem.Id);
        if (existingItem == null)
        {
            throw new InvalidOperationException($"TodoItem with ID {todoItem.Id} not found");
        }
        
        try
        {
            _context.Entry(existingItem).CurrentValues.SetValues(todoItem);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.TodoItems.AnyAsync(e => e.Id == todoItem.Id))
            {
                throw new InvalidOperationException($"TodoItem with ID {todoItem.Id} not found");
            }
            else
            {
                throw new InvalidOperationException($"TodoItem with ID {todoItem.Id} was modified by another user");
            }
        }
    }

    public async Task DeleteAsync(long id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem != null)
        {
            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.TodoItems.AnyAsync(e => e.Id == id);
    }
}