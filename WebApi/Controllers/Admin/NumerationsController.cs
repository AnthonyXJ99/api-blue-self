using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueSelfCheckout.WebApi.Models.Admin;
using BlueSelfCheckout.Data;

namespace BlueSelfCheckout.WebApi.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class NumerationsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public NumerationsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Numerations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Numeration>>> GetNumeration()
        {
            return await _context.Numeration.ToListAsync();
        }

        // GET: api/Numerations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Numeration>> GetNumeration(string id)
        {
            var numeration = await _context.Numeration.FindAsync(id);

            if (numeration == null)
            {
                return NotFound();
            }

            return numeration;
        }

        // PUT: api/Numerations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNumeration(string id, Numeration numeration)
        {
            if (id != numeration.ObjectCode)
            {
                return BadRequest();
            }

            _context.Entry(numeration).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NumerationExists(id))
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

        // POST: api/Numerations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Numeration>> PostNumeration(Numeration numeration)
        {
            _context.Numeration.Add(numeration);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (NumerationExists(numeration.ObjectCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetNumeration", new { id = numeration.ObjectCode }, numeration);
        }

        // DELETE: api/Numerations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNumeration(string id)
        {
            var numeration = await _context.Numeration.FindAsync(id);
            if (numeration == null)
            {
                return NotFound();
            }

            _context.Numeration.Remove(numeration);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NumerationExists(string id)
        {
            return _context.Numeration.Any(e => e.ObjectCode == id);
        }
    }
}
