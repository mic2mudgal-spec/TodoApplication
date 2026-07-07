using Microsoft.AspNetCore.Mvc;
using Todo.Application.DTOs.Requests;
using Todo.Application.Interfaces;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TasksController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _todoService.GetAllAsync();
        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var task = await _todoService.GetByIdAsync(id);

        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTodoTaskRequest request)
    {
        var task = await _todoService.CreateAsync(request);

        return CreatedAtAction(
            nameof(Get),
            new { id = task.Id },
            task);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateTodoTaskRequest request)
    {
        if (id != request.Id)
            return BadRequest("Id mismatch.");

        await _todoService.UpdateAsync(request);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _todoService.DeleteAsync(id);

        return NoContent();
    }
}