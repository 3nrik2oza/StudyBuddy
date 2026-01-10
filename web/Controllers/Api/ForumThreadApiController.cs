using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Models.Entities;

namespace web.Controllers_Api
{
    [Route("api/v1/ForumThread")]
    [ApiController]
    public class ForumThreadApiController : ControllerBase
    {
        private readonly StudyBuddyDbContext _context;

        public ForumThreadApiController(StudyBuddyDbContext context)
        {
            _context = context;
        }

        // GET: api/ForumThreadApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ForumThread>>> GetForumThreads()
        {
            return await _context.ForumThreads.ToListAsync();
        }

        // GET: api/ForumThreadApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ForumThread>> GetForumThread(int id)
        {
            var forumThread = await _context.ForumThreads.FindAsync(id);

            if (forumThread == null)
            {
                return NotFound();
            }

            return forumThread;
        }

        // PUT: api/ForumThreadApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutForumThread(int id, ForumThread forumThread)
        {
            if (id != forumThread.Id)
            {
                return BadRequest();
            }

            _context.Entry(forumThread).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ForumThreadExists(id))
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

        // POST: api/ForumThreadApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ForumThread>> PostForumThread(ForumThread forumThread)
        {
            _context.ForumThreads.Add(forumThread);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetForumThread", new { id = forumThread.Id }, forumThread);
        }

        // DELETE: api/ForumThreadApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForumThread(int id)
        {
            var forumThread = await _context.ForumThreads.FindAsync(id);
            if (forumThread == null)
            {
                return NotFound();
            }

            _context.ForumThreads.Remove(forumThread);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ForumThreadExists(int id)
        {
            return _context.ForumThreads.Any(e => e.Id == id);
        }
    }
}
