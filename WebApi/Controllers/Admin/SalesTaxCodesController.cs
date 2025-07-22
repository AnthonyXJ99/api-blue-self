using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueSelfCheckout.Data;
using BlueSelfCheckout.WebApi.Models.Admin;

namespace BlueSelfCheckout.WebApi.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesTaxCodesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public SalesTaxCodesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/SalesTaxCodes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesTaxCodes>>> GetSalesTaxCodes()
        {
            return await _context.SalesTaxCodes.ToListAsync();
        }

        // GET: api/SalesTaxCodes/5
        [HttpGet("{taxCode}")]
        public async Task<ActionResult<SalesTaxCodes>> GetSalesTaxCodes(string taxCode)
        {
            var salesTaxCodes = await _context.SalesTaxCodes.FindAsync(taxCode);

            if (salesTaxCodes == null)
            {
                return NotFound();
            }

            return salesTaxCodes;
        }

        // PUT: api/SalesTaxCodes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{taxCode}")]
        public async Task<IActionResult> PutSalesTaxCodes(string taxCode, SalesTaxCodes salesTaxCodes)
        {
            if (taxCode != salesTaxCodes.TaxCode)
            {
                return BadRequest();
            }

            _context.Entry(salesTaxCodes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesTaxCodesExists(taxCode))
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

        // POST: api/SalesTaxCodes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SalesTaxCodes>> PostSalesTaxCodes(SalesTaxCodes salesTaxCodes)
        {
            _context.SalesTaxCodes.Add(salesTaxCodes);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SalesTaxCodesExists(salesTaxCodes.TaxCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSalesTaxCodes", new { taxCode = salesTaxCodes.TaxCode }, salesTaxCodes);
        }

        // DELETE: api/SalesTaxCodes/5
        [HttpDelete("{taxCode}")]
        public async Task<IActionResult> DeleteSalesTaxCodes(string taxCode)
        {
            var salesTaxCodes = await _context.SalesTaxCodes.FindAsync(taxCode);
            if (salesTaxCodes == null)
            {
                return NotFound();
            }

            _context.SalesTaxCodes.Remove(salesTaxCodes);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SalesTaxCodesExists(string taxCode)
        {
            return _context.SalesTaxCodes.Any(e => e.TaxCode == taxCode);
        }
    }// fin de la clase

}// fin del namespace
