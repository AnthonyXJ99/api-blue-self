using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueSelfCheckout.Data;
using BlueSelfCheckout.WebApi.Models.Customers;
using BlueSelfCheckout.WebApi.Models;

namespace BlueSelfCheckout.WebApi.Controllers.Customers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerGroupsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public CustomerGroupsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/CustomerGroups/all 
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<CustomerGroup>>> GetAllCustomerGroup()
        {
            return await _context.CustomerGroup.ToListAsync();
        }

        // GET: api/CustomerGroups
        [HttpGet]
        public async Task<ActionResult<PagedResponse<Customer>>> GetCustomerGroup(
            int pageNumber = 1,
            int pageSize = 10,
            string search = null) // Parámetro de búsqueda (search)
        {
            try
            {
                // Comenzamos la consulta con todos los grupos de clientes
                IQueryable<CustomerGroup> query = _context.CustomerGroup;

                // Si se proporciona un término de búsqueda, filtramos los resultados
                if (!string.IsNullOrEmpty(search))
                {
                    // Realizamos una búsqueda que sea insensible a mayúsculas y minúsculas en el nombre del grupo de cliente
                    query = query.Where(c => c.CustomerGroupCode.Contains(search) || c.CustomerGroupName.Contains(search) );
                }

                // Calcular el total de grupo de clientes después de aplicar el filtro
                var totalCount = await query.CountAsync();

                // Obtener la página de datos solicitada
                var customersGroup = await query
                    .Skip((pageNumber - 1) * pageSize)  // Saltar los primeros (pageNumber - 1) * pageSize registros
                    .Take(pageSize)                     // Tomar solo 'pageSize' registros
                    .ToListAsync();

                // Crear la respuesta paginada
                var response = new PagedResponse<CustomerGroup>(totalCount, pageNumber, pageSize, customersGroup);

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Loguear la excepción (puedes usar tu propia forma de registrar los errores)
                Console.Error.WriteLine($"Error al obtener los grupos de clientes: {ex.Message}");

                // Devolver un mensaje de error más detallado
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }


        // GET: api/CustomerGroups/5
        [HttpGet("{customerGroupCode}")]
        public async Task<ActionResult<CustomerGroup>> GetCustomerGroup(string customerGroupCode)
        {
            var customerGroup = await _context.CustomerGroup.FindAsync(customerGroupCode);

            if (customerGroup == null)
            {
                return NotFound();
            }

            return customerGroup;
        }

        // PUT: api/CustomerGroups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{customerGroupCode}")]
        public async Task<IActionResult> PutCustomerGroup(string customerGroupCode, CustomerGroup customerGroup)
        {
            if (customerGroupCode != customerGroup.CustomerGroupCode)
            {
                return BadRequest();
            }

            _context.Entry(customerGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerGroupExists(customerGroupCode))
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

        // POST: api/CustomerGroups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CustomerGroup>> PostCustomerGroup([FromBody] CustomerGroup customerGroup)
        {
            _context.CustomerGroup.Add(customerGroup);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerGroupExists(customerGroup.CustomerGroupCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCustomerGroup", new { id = customerGroup.CustomerGroupCode }, customerGroup);
        }

        // DELETE: api/CustomerGroups/5
        [HttpDelete("{customerGroupCode}")]
        public async Task<IActionResult> DeleteCustomerGroup(string customerGroupCode)
        {
            var customerGroup = await _context.CustomerGroup.FindAsync(customerGroupCode);
            if (customerGroup == null)
            {
                return NotFound();
            }

            _context.CustomerGroup.Remove(customerGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerGroupExists(string customerGroupCode)
        {
            return _context.CustomerGroup.Any(e => e.CustomerGroupCode == customerGroupCode);
        }

    }
}
