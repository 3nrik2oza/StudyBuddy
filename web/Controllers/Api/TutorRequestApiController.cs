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
    [Route("api/v1/TutorRequest")]
    [ApiController]
    public class TutorRequestApiController : ControllerBase
    {
        private readonly StudyBuddyDbContext _context;

        public TutorRequestApiController(StudyBuddyDbContext context)
        {
            _context = context;
        }

        // GET: api/TutorRequestApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TutorRequest>>> GetTutorRequests()
        {
            return await _context.TutorRequests.ToListAsync();
        }

        // GET: api/TutorRequestApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TutorRequest>> GetTutorRequest(int id)
        {
            var tutorRequest = await _context.TutorRequests.FindAsync(id);

            if (tutorRequest == null)
            {
                return NotFound();
            }

            return tutorRequest;
        }

        // PUT: api/TutorRequestApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTutorRequest(int id, TutorRequest tutorRequest)
        {
            if (id != tutorRequest.Id)
            {
                return BadRequest();
            }

            _context.Entry(tutorRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TutorRequestExists(id))
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

        // POST: api/TutorRequestApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TutorRequest>> PostTutorRequest(TutorRequest tutorRequest)
        {
            _context.TutorRequests.Add(tutorRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTutorRequest", new { id = tutorRequest.Id }, tutorRequest);
        }

        // DELETE: api/TutorRequestApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTutorRequest(int id)
        {
            var tutorRequest = await _context.TutorRequests.FindAsync(id);
            if (tutorRequest == null)
            {
                return NotFound();
            }

            _context.TutorRequests.Remove(tutorRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TutorRequestExists(int id)
        {
            return _context.TutorRequests.Any(e => e.Id == id);
        }
    }
}
