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
    [Route("api/v1/TutorSubject")]
    [ApiController]
    public class TutorSubjectApiController : ControllerBase
    {
        private readonly StudyBuddyDbContext _context;

        public TutorSubjectApiController(StudyBuddyDbContext context)
        {
            _context = context;
        }

        // GET: api/TutorSubjectApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TutorSubject>>> GetTutorSubjects()
        {
            return await _context.TutorSubjects.ToListAsync();
        }

        // GET: api/TutorSubjectApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TutorSubject>> GetTutorSubject(string id)
        {
            var tutorSubject = await _context.TutorSubjects.FindAsync(id);

            if (tutorSubject == null)
            {
                return NotFound();
            }

            return tutorSubject;
        }

        // PUT: api/TutorSubjectApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
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

        // POST: api/TutorSubjectApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
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

        // DELETE: api/TutorSubjectApi/5
        [HttpDelete("{id}")]
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
