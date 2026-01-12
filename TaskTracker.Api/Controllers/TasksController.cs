using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using TaskTracker.Api.Models;
using System;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    //Returns All Tasks - options for sort order
    [HttpGet]
    public async Task<IActionResult> GetTasks(
        [FromQuery] string sortBy = "createdAt",
        [FromQuery] string order = "desc")
    {
        Console.WriteLine("GetTasks Called");
        IQueryable<TaskItem> query = _context.Tasks;

        query = (sortBy, order.ToLower()) switch
        {
            ("createdAt", "asc") => query.OrderBy(t => t.CreatedAt),
            ("createdAt", "desc") => query.OrderByDescending(t => t.CreatedAt),
            ("priority", "asc") => query.OrderBy(t => t.Priority),
            ("priority", "desc") => query.OrderByDescending(t => t.Priority),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };

        var tasks = await query.ToListAsync();
        return Ok(tasks);
    }

    //Returns Task By ID
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetTask(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        return Ok(task);
    }

    //Creates task and saves to DB
    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    //Updates existing Task, by ID
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(Guid id, TaskItem updatedTask)
    {
        if (id != updatedTask.Id) return BadRequest();

        _context.Entry(updatedTask).State = EntityState.Modified;

       try
       {
           await _context.SaveChangesAsync();
       }
       catch (DbUpdateConcurrencyException)
       {
           if (!_context.Tasks.Any(t => t.Id == id)) return NotFound();
           throw;
       }

        return NoContent();
    }

    //Deletes Task by ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
}