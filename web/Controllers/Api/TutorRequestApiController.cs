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
    [Route("api/v1/TutorRequest")]
    [ApiController]
    public class TutorRequestApiController : ControllerBase
    {
        private readonly StudyBuddyDbContext _context;

        public TutorRequestApiController(StudyBuddyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ApiKeyAuth]
        public async Task<ActionResult<IEnumerable<TutorRequest>>> GetTutorRequests()
        {
            return await _context.TutorRequests.ToListAsync();
        }

        [HttpGet("{id}")]
        [ApiKeyAuth]
        public async Task<ActionResult<TutorRequest>> GetTutorRequest(int id)
        {
            var tutorRequest = await _context.TutorRequests.FindAsync(id);

            if (tutorRequest == null)
            {
                return NotFound();
            }

            return tutorRequest;
        }

        [HttpPut("{id}")]
        [ApiKeyAuth]
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

        [HttpPost]
        [ApiKeyAuth]
        public async Task<ActionResult<TutorRequest>> PostTutorRequest(TutorRequest tutorRequest)
        {
            _context.TutorRequests.Add(tutorRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTutorRequest", new { id = tutorRequest.Id }, tutorRequest);
        }

        [HttpDelete("{id}")]
        [ApiKeyAuth]
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
