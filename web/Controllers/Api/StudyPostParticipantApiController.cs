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
    [Route("api/v1/StudyPostParticipant")]
    [ApiController]
    public class StudyPostParticipantApiController : ControllerBase
    {
        private readonly StudyBuddyDbContext _context;

        public StudyPostParticipantApiController(StudyBuddyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ApiKeyAuth]
        public async Task<ActionResult<IEnumerable<StudyPostParticipant>>> GetStudyPostParticipants()
        {
            return await _context.StudyPostParticipants.ToListAsync();
        }

        [HttpGet("{id}")]
        [ApiKeyAuth]
        public async Task<ActionResult<StudyPostParticipant>> GetStudyPostParticipant(int id)
        {
            var studyPostParticipant = await _context.StudyPostParticipants.FindAsync(id);

            if (studyPostParticipant == null)
            {
                return NotFound();
            }

            return studyPostParticipant;
        }

        [HttpPut("{id}")]
        [ApiKeyAuth]
        public async Task<IActionResult> PutStudyPostParticipant(int id, StudyPostParticipant studyPostParticipant)
        {
            if (id != studyPostParticipant.Id)
            {
                return BadRequest();
            }

            _context.Entry(studyPostParticipant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudyPostParticipantExists(id))
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
        public async Task<ActionResult<StudyPostParticipant>> PostStudyPostParticipant(StudyPostParticipant studyPostParticipant)
        {
            _context.StudyPostParticipants.Add(studyPostParticipant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudyPostParticipant", new { id = studyPostParticipant.Id }, studyPostParticipant);
        }

        [HttpDelete("{id}")]
        [ApiKeyAuth]
        public async Task<IActionResult> DeleteStudyPostParticipant(int id)
        {
            var studyPostParticipant = await _context.StudyPostParticipants.FindAsync(id);
            if (studyPostParticipant == null)
            {
                return NotFound();
            }

            _context.StudyPostParticipants.Remove(studyPostParticipant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudyPostParticipantExists(int id)
        {
            return _context.StudyPostParticipants.Any(e => e.Id == id);
        }
    }
}
