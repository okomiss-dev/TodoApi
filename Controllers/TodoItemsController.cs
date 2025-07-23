using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers
{
    [Route("api/TodoItems")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoRepository _repository;
        private readonly IMapper _mapper;

        public TodoItemsController(ITodoRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            var todoItems = await _repository.GetAllAsync();
            var todoItemDTOs = _mapper.Map<IEnumerable<TodoItemDTO>>(todoItems);
            return Ok(todoItemDTOs);
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItem = await _repository.GetByIdAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return _mapper.Map<TodoItemDTO>(todoItem);
        }

        // PUT: api/TodoItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            if (id != todoItemDTO.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                await _repository.UpdateAsync(_mapper.Map<TodoItem>(todoItemDTO));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("modified by another user"))
            {
                return Conflict("The item was modified by another user. Please refresh and try again.");
            }

            return NoContent();
        }

        // POST: api/TodoItems
        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoItemDTO)
        {
            try
            {
                var todoItem = _mapper.Map<TodoItem>(todoItemDTO);
                await _repository.CreateAsync(todoItem);
                var resultDTO = _mapper.Map<TodoItemDTO>(todoItem);
                return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, resultDTO);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(500, "Failed to create TodoItem");
            }
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var exists = await TodoItemExists(id);
            if (!exists)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            return NoContent();
        }

        private async Task<bool> TodoItemExists(long id)
        {
            return await _repository.ExistsAsync(id);
        }
    }
}
