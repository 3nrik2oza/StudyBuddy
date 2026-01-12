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
    [Route("api/v1/TutorSubject")]
    [ApiController]
    public class TutorSubjectApiController : ControllerBase
    {
        private readonly StudyBuddyDbContext _context;

        public TutorSubjectApiController(StudyBuddyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ApiKeyAuth]
        public async Task<ActionResult<IEnumerable<TutorSubject>>> GetTutorSubjects()
        {
            return await _context.TutorSubjects.ToListAsync();
        }

        [HttpGet("{id}")]
        [ApiKeyAuth]
        public async Task<ActionResult<TutorSubject>> GetTutorSubject(string id)
        {
            var tutorSubject = await _context.TutorSubjects.FindAsync(id);

            if (tutorSubject == null)
            {
                return NotFound();
            }

            return tutorSubject;
        }

        [HttpPut("{id}")]
        [ApiKeyAuth]
        public async Task<IActionResult> PutTutorSubject(string id, TutorSubject tutorSubject)
        {
            if (id != tutorSubject.UserId)
            {
                return BadRequest();
            }

            _context.Entry(tutorSubject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TutorSubjectExists(id))
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
        public async Task<ActionResult<TutorSubject>> PostTutorSubject(TutorSubject tutorSubject)
        {
            _context.TutorSubjects.Add(tutorSubject);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TutorSubjectExists(tutorSubject.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTutorSubject", new { id = tutorSubject.UserId }, tutorSubject);
        }

        [HttpDelete("{id}")]
        [ApiKeyAuth]
        public async Task<IActionResult> DeleteTutorSubject(string id)
        {
            var tutorSubject = await _context.TutorSubjects.FindAsync(id);
            if (tutorSubject == null)
            {
                return NotFound();
            }

            _context.TutorSubjects.Remove(tutorSubject);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TutorSubjectExists(string id)
        {
            return _context.TutorSubjects.Any(e => e.UserId == id);
        }
    }
}
