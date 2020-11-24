using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskApi.Models;
using static TaskApi.Enums.Emuns;

namespace TaskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemsController : ControllerBase
    {
        private readonly TaskContext _context;

        public TaskItemsController(TaskContext context)
        {
            _context = context;
        }


        [HttpPost("CreateTask")]
        public async Task<ActionResult<TaskItem>> CreateTaskToOn(string taskname, User assignedTo, DateTime? scheduled)
        {

            var taskItem = new TaskItem
            {
                Name = taskname,
                AssignedTo = assignedTo,
                State = State.Scheduled,
                StratDate = scheduled ?? DateTime.Now,
                InProgressDate = null,
                EndDate = null,               
            };

            _context.TaskItem.Add(taskItem);

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTaskItem), new { id = taskItem.Id }, taskItem);
        }

        [HttpPut("PutTaskInProgress/{taskId}")]
        public async Task<IActionResult> PutTaskInProgress(int taskId)
        {
            var taskItem = await _context.TaskItem.FindAsync(taskId);

            if (taskItem == null)           
                return NotFound();
            
               
            _context.Entry(taskItem).State = EntityState.Modified;
            taskItem.State = State.InProgress;
            taskItem.InProgressDate = DateTime.Now;
            taskItem.EndDate = null;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskItemExists(taskId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpPut("InProgressTime/{taskId}")]
        public async Task<ActionResult<string>> GetInProgressTime(int taskId)
        {
            var taskItem = await _context.TaskItem.FindAsync(taskId);

            if (taskItem == null && taskItem.InProgressDate == null)
            {
                return NotFound();
            }

            var EndDate = taskItem.EndDate ?? DateTime.Now;

            int count = taskItem.InProgressDate.Value.Date.Subtract(EndDate.Date).Duration().Days + 1;

            return  $"In Progress for {count} days";
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTaskItem()
        {
            return await _context.TaskItem.ToListAsync();
        }

        // GET: api/TaskItems/5
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskItem(int id)
        {
            var taskItem = await _context.TaskItem.FindAsync(id);

            if (taskItem == null)
            {
                return NotFound();
            }

            return taskItem;
        }

      

        private bool TaskItemExists(int id)
        {
            return _context.TaskItem.Any(e => e.Id == id);
        }
    }
}
