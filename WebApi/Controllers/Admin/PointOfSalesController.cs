using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueSelfCheckout.WebApi.Models.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueSelfCheckout.Data;
using BlueSelfCheckout.WebApi.Models.Customers;
using BlueSelfCheckout.WebApi.Models;

namespace BlueSelfCheckout.WebApi.Controllers.Admin
{
    /// <summary>
    /// Controlador que gestiona los (POS) de redelcom y transbank en el sistema.
    /// Permite obtener, crear, actualizar y eliminar POS.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PointOfSalesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Inicializa una nueva instancia del controlador <see cref="PointOfSalesController"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>
        public PointOfSalesController(ApplicationDBContext context)
        {
            _context = context;
        }

       
        /// <summary>
        /// Obtiene una lista de todos los (POS) registrados.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="PointOfSale"/>.</returns>
        // GET: api/PointOfSales/all 
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<PointOfSale>>> GetPointOfSale()
        {
            return await _context.PointOfSale.ToListAsync();
        }


        // GET: api/PointOfSales
        [HttpGet]
        public async Task<ActionResult<PagedResponse<PointOfSale>>> GetPointOfSale(
            int pageNumber = 1,
            int pageSize = 10,
            string search = null) // Parámetro de búsqueda (search)
        {
            try
            {
                // Comenzamos la consulta con todos los puntos de venta
                IQueryable<PointOfSale> query = _context.PointOfSale;

                // Si se proporciona un término de búsqueda, filtramos los resultados
                if (!string.IsNullOrEmpty(search))
                {
                    // Realizamos una búsqueda que sea insensible a mayúsculas y minúsculas en el nombre del grupo de cliente
                    query = query.Where(p => p.PosCode.Contains(search) || p.PosName.Contains(search));
                }

                // Calcular el total de puntos de venta después de aplicar el filtro
                var totalCount = await query.CountAsync();

                // Obtener la página de datos solicitada
                var pointOfSale = await query
                    .Skip((pageNumber - 1) * pageSize)  // Saltar los primeros (pageNumber - 1) * pageSize registros
                    .Take(pageSize)                     // Tomar solo 'pageSize' registros
                    .ToListAsync();

                // Crear la respuesta paginada
                var response = new PagedResponse<PointOfSale>(totalCount, pageNumber, pageSize, pointOfSale);

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Loguear la excepción (puedes usar tu propia forma de registrar los errores)
                Console.Error.WriteLine($"Error al obtener los puntos de venta: {ex.Message}");

                // Devolver un mensaje de error más detallado
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }


        // GET: api/PointOfSales/5
        /// <summary>
        /// Obtiene un POS específico utilizando el código del POS como parámetro de búsqueda.
        /// </summary>
        /// <param name="posCode">El código único del POS.</param>
        /// <returns>El POS correspondiente al código proporcionado o un error si no se encuentra.</returns>
        [HttpGet("{posCode}")]
        public async Task<ActionResult<PointOfSale>> GetPointOfSale(string posCode)
        {
            var pointOfSale = await _context.PointOfSale.FindAsync(posCode);

            if (pointOfSale == null)
            {
                return NotFound();
            }

            return pointOfSale;
        }

        // PUT: api/PointOfSales/5
        /// <summary>
        /// Actualiza los datos de un POS existente.
        /// </summary>
        /// <param name="posCode">El código del POS a actualizar.</param>
        /// <param name="pointOfSale">Los nuevos datos del POS.</param>
        /// <returns>Un resultado indicando el éxito o el error de la operación.</returns>
        [HttpPut("{posCode}")]
        public async Task<IActionResult> PutPointOfSale(string posCode, [FromBody] PointOfSale pointOfSale)
        {
            if (posCode != pointOfSale.PosCode)
            {
                return BadRequest();
            }
            _context.Entry(pointOfSale).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PointOfSaleExists(posCode))
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

        // POST: api/PointOfSales
        /// <summary>
        /// Crea un nuevo POS en el sistema.
        /// </summary>
        /// <param name="pointOfSale">El POS a crear.</param>
        /// <returns>El POS creado y la ubicación del recurso.</returns>
        [HttpPost]
        public async Task<ActionResult<PointOfSale>> PostPointOfSale(PointOfSale pointOfSale)
        {
            _context.PointOfSale.Add(pointOfSale);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PointOfSaleExists(pointOfSale.PosCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPointOfSale", new { posCode = pointOfSale.PosCode }, pointOfSale);
        }

        // DELETE: api/PointOfSales/5
        /// <summary>
        /// Elimina un POS existente.
        /// </summary>
        /// <param name="posCode">El código del POS a eliminar.</param>
        /// <returns>Un resultado indicando el éxito o el error de la operación.</returns>
        [HttpDelete("{posCode}")]
        public async Task<IActionResult> DeletePointOfSale(string posCode)
        {
            var pointOfSale = await _context.PointOfSale.FindAsync(posCode);
            if (pointOfSale == null)
            {
                return NotFound();
            }

            _context.PointOfSale.Remove(pointOfSale);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica si un POS con el código especificado existe en la base de datos.
        /// </summary>
        /// <param name="posCode">El código del POS a verificar.</param>
        /// <returns>Verdadero si el POS existe; falso en caso contrario.</returns>
        private bool PointOfSaleExists(string posCode)
        {
            return _context.PointOfSale.Any(e => e.PosCode == posCode);
        }


    }// fin de la clase
}// fin del namespace
