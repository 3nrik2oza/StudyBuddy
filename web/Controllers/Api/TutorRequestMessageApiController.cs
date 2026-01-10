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
    [Route("api/v1/TutorRequestMessage")]
    [ApiController]
    public class TutorRequestMessageApiController : ControllerBase
    {
        private readonly StudyBuddyDbContext _context;

        public TutorRequestMessageApiController(StudyBuddyDbContext context)
        {
            _context = context;
        }

        // GET: api/TutorRequestMessageApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TutorRequestMessage>>> GetTutorRequestMessages()
        {
            return await _context.TutorRequestMessages.ToListAsync();
        }

        // GET: api/TutorRequestMessageApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TutorRequestMessage>> GetTutorRequestMessage(int id)
        {
            var tutorRequestMessage = await _context.TutorRequestMessages.FindAsync(id);

            if (tutorRequestMessage == null)
            {
                return NotFound();
            }

            return tutorRequestMessage;
        }

        // PUT: api/TutorRequestMessageApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTutorRequestMessage(int id, TutorRequestMessage tutorRequestMessage)
        {
            if (id != tutorRequestMessage.Id)
            {
                return BadRequest();
            }

            _context.Entry(tutorRequestMessage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TutorRequestMessageExists(id))
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

        // POST: api/TutorRequestMessageApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TutorRequestMessage>> PostTutorRequestMessage(TutorRequestMessage tutorRequestMessage)
        {
            _context.TutorRequestMessages.Add(tutorRequestMessage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTutorRequestMessage", new { id = tutorRequestMessage.Id }, tutorRequestMessage);
        }

        // DELETE: api/TutorRequestMessageApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTutorRequestMessage(int id)
        {
            var tutorRequestMessage = await _context.TutorRequestMessages.FindAsync(id);
            if (tutorRequestMessage == null)
            {
                return NotFound();
            }

            _context.TutorRequestMessages.Remove(tutorRequestMessage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TutorRequestMessageExists(int id)
        {
            return _context.TutorRequestMessages.Any(e => e.Id == id);
        }
    }
}
