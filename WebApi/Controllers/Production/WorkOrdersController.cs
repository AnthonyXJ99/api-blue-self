using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueSelfCheckout.Data;
using BlueSelfCheckout.WebApi.Models.Production;
using BlueSelfCheckout.WebApi.Models.Products;

namespace BlueSelfCheckout.WebApi.Controllers.Production
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrdersController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public WorkOrdersController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/WorkOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrder>>> GetWorkOrder()
        {
            return await _context.WorkOrder.ToListAsync();
        }

        // GET: api/WorkOrders/5
        [HttpGet("{docEntry}")]
        public async Task<ActionResult<WorkOrder>> GetWorkOrder(int docEntry)
        {
            var workOrder = await _context.WorkOrder.FindAsync(docEntry);

            if (workOrder == null)
            {
                return NotFound();
            }

            return workOrder;
        }

        // PUT: api/WorkOrders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{docEntry}")]
        public async Task<IActionResult> PutWorkOrder(int docEntry, WorkOrder workOrder)
        {
            if (docEntry != workOrder.DocEntry)
            {
                return BadRequest();
            }

            _context.Entry(workOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkOrderExists(docEntry))
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

        // POST: api/WorkOrders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<WorkOrder>> PostWorkOrder(WorkOrder workOrder)
        {
            _context.WorkOrder.Add(workOrder);

            // Si la orden de producccion tiene los materiales, agregarlos
            if (workOrder.Items != null && workOrder.Items.Any())
            {
                foreach (var item in workOrder.Items)
                {
                    item.WorkOrderDocEntry = workOrder.DocEntry;  // Asociar el material con la orden de produccion
                    _context.WorkOrderItem.Add(item);  // Agregar material a la base de datos
                }
            }
            

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkOrder", new { docEntry = workOrder.DocEntry }, workOrder);
        }

        // DELETE: api/WorkOrders/5
        [HttpDelete("{docEntry}")]
        public async Task<IActionResult> DeleteWorkOrder(int docEntry)
        {
            var workOrder = await _context.WorkOrder.FindAsync(docEntry);
            if (workOrder == null)
            {
                return NotFound();
            }

            _context.WorkOrder.Remove(workOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorkOrderExists(int docEntry)
        {
            return _context.WorkOrder.Any(e => e.DocEntry == docEntry);
        }



    }// fin de la clase
} // fin del namespace
