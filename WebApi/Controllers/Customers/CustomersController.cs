using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueSelfCheckout.WebApi.Models;
using BlueSelfCheckout.WebApi.Models.Customers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueSelfCheckout.Data;


namespace BlueSelfCheckout.WebApi.Controllers.Customers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public CustomersController(ApplicationDBContext context)
        {
            _context = context;
        }

        //// GET: api/Customers
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer()
        //{
        //    return await _context.Customer.ToListAsync();
        //}

        // GET: api/Customers/5
        [HttpGet("{customerCode}")]
        public async Task<ActionResult<Customer>> GetCustomer(string customerCode)
        {
            var customer = await _context.Customer.FindAsync(customerCode);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // GET: api/Customers/all 
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            return await _context.Customer.ToListAsync();
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<PagedResponse<Customer>>> GetCustomers(
            int pageNumber = 1,
            int pageSize = 10,
            string search = null) // Parámetro de búsqueda (search)
        {
            try
            {
                // Comenzamos la consulta con todos los clientes
                IQueryable<Customer> query = _context.Customer;

                // Si se proporciona un término de búsqueda, filtramos los resultados
                if (!string.IsNullOrEmpty(search))
                {
                    // Realizamos una búsqueda que sea insensible a mayúsculas y minúsculas en el nombre y correo electrónico
                    query = query.Where(c => c.CustomerName.Contains(search) || c.Email.Contains(search) || c.TaxIdentNumber.Contains(search));
                }

                // Calcular el total de clientes después de aplicar el filtro
                var totalCount = await query.CountAsync();

                // Obtener la página de datos solicitada
                var customers = await query
                    .Skip((pageNumber - 1) * pageSize)  // Saltar los primeros (pageNumber - 1) * pageSize registros
                    .Take(pageSize)                     // Tomar solo 'pageSize' registros
                    .ToListAsync();

                // Crear la respuesta paginada
                var response = new PagedResponse<Customer>(totalCount, pageNumber, pageSize, customers);

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Loguear la excepción (puedes usar tu propia forma de registrar los errores)
                Console.Error.WriteLine($"Error al obtener los clientes: {ex.Message}");

                // Devolver un mensaje de error más detallado
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }




        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{customerCode}")]
        public async Task<IActionResult> PutCustomer(string customerCode, [FromBody] Customer customer)
        {
            if (customerCode != customer.CustomerCode)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customerCode))
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

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer([FromBody] Customer customer)
        {
            _context.Customer.Add(customer);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerExists(customer.CustomerCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetCustomer), new { customerCode = customer.CustomerCode }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{customerCode}")]
        public async Task<IActionResult> DeleteCustomer(string customerCode)
        {
            var customer = await _context.Customer.FindAsync(customerCode);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(string customerCode)
        {
            return _context.Customer.Any(e => e.CustomerCode == customerCode);
        }
    }
}
