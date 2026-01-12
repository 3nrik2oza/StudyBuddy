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
    [Route("api/v1/StudyPost")]
    [ApiController]
    public class StudyPostApiController : ControllerBase
    {
        private readonly StudyBuddyDbContext _context;

        public StudyPostApiController(StudyBuddyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ApiKeyAuth]
        public async Task<ActionResult<IEnumerable<StudyPost>>> GetStudyPosts()
        {
            return await _context.StudyPosts.ToListAsync();
        }

        [HttpGet("{id}")]
        [ApiKeyAuth]
        public async Task<ActionResult<StudyPost>> GetStudyPost(int id)
        {
            var studyPost = await _context.StudyPosts.FindAsync(id);

            if (studyPost == null)
            {
                return NotFound();
            }

            return studyPost;
        }

        [HttpPut("{id}")]
        [ApiKeyAuth]
        public async Task<IActionResult> PutStudyPost(int id, StudyPost studyPost)
        {
            if (id != studyPost.Id)
            {
                return BadRequest();
            }

            _context.Entry(studyPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudyPostExists(id))
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
        public async Task<ActionResult<StudyPost>> PostStudyPost(StudyPost studyPost)
        {
            _context.StudyPosts.Add(studyPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudyPost", new { id = studyPost.Id }, studyPost);
        }

        [HttpDelete("{id}")]
        [ApiKeyAuth]
        public async Task<IActionResult> DeleteStudyPost(int id)
        {
            var studyPost = await _context.StudyPosts.FindAsync(id);
            if (studyPost == null)
            {
                return NotFound();
            }

            _context.StudyPosts.Remove(studyPost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudyPostExists(int id)
        {
            return _context.StudyPosts.Any(e => e.Id == id);
        }
    }
}
