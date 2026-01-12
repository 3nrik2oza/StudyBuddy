using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Models.Entities;
using web.Filters;

namespace web.Controllers_Api
{
    [Route("api/v1/ForumReply")]
    [ApiController]
    public class ForumReplyApiController : ControllerBase
    {
        private readonly StudyBuddyDbContext _context;

        public ForumReplyApiController(StudyBuddyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ApiKeyAuth]
        public async Task<ActionResult<IEnumerable<ForumReply>>> GetForumReplies()
        {
            return await _context.ForumReplies.ToListAsync();
        }

        [HttpGet("{id}")]
        [ApiKeyAuth]
        public async Task<ActionResult<ForumReply>> GetForumReply(int id)
        {
            var forumReply = await _context.ForumReplies.FindAsync(id);

            if (forumReply == null)
            {
                return NotFound();
            }

            return forumReply;
        }

        [HttpPut("{id}")]
        [ApiKeyAuth]
        public async Task<IActionResult> PutForumReply(int id, ForumReply forumReply)
        {
            if (id != forumReply.Id)
            {
                return BadRequest();
            }

            _context.Entry(forumReply).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ForumReplyExists(id))
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

        [HttpPost]
        [ApiKeyAuth]
        public async Task<ActionResult<ForumReply>> PostForumReply(ForumReply forumReply)
        {
            _context.ForumReplies.Add(forumReply);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetForumReply", new { id = forumReply.Id }, forumReply);
        }

        [HttpDelete("{id}")]
        [ApiKeyAuth]
        public async Task<IActionResult> DeleteForumReply(int id)
        {
            var forumReply = await _context.ForumReplies.FindAsync(id);
            if (forumReply == null)
            {
                return NotFound();
            }

            _context.ForumReplies.Remove(forumReply);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ForumReplyExists(int id)
        {
            return _context.ForumReplies.Any(e => e.Id == id);
        }
    }
}
