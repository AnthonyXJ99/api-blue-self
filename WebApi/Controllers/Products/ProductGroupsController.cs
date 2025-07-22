using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueSelfCheckout.WebApi.Models.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueSelfCheckout.Data;
using BlueSelfCheckout.WebApi.Models.Customers;
using BlueSelfCheckout.WebApi.Models;

namespace BlueSelfCheckout.WebApi.Controllers.Products
{
    /// <summary>
    /// Controlador que gestiona los grupos de productos en el sistema.
    /// Permite obtener, crear, actualizar y eliminar grupos de productos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductGroupsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Inicializa una nueva instancia del controlador <see cref="ProductGroupsController"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>
        public ProductGroupsController(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene una lista de todos los grupos de productos.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="ProductGroup"/>.</returns>
       
        // GET: api/ProductGroups/all 
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ProductGroup>>> GetAllProductGroup()
        {
            return await _context.ProductGroup.ToListAsync();
        }


        // GET: api/ProductGroups
        [HttpGet]
        public async Task<ActionResult<PagedResponse<ProductGroup>>> GetProductGroup(
            int pageNumber = 1,
            int pageSize = 10,
            string search = null) // Parámetro de búsqueda (search)
        {
            try
            {
                // Comenzamos la consulta con todos los grupos de productos
                IQueryable<ProductGroup> query = _context.ProductGroup;
 
                // Si se proporciona un término de búsqueda, filtramos los resultados
                if (!string.IsNullOrEmpty(search))
                {
                    // Realizamos una búsqueda que sea insensible a mayúsculas y minúsculas en el código y  nombre del grupo de producto
                    query = query.Where(pg => pg.ProductGroupCode.Contains(search) || pg.ProductGroupName.Contains(search));
                }

                // Calcular el total de grupo de productos después de aplicar el filtro
                var totalCount = await query.CountAsync();

                // Obtener la página de datos solicitada
                var productsGroup = await query
                    .Skip((pageNumber - 1) * pageSize)  // Saltar los primeros (pageNumber - 1) * pageSize registros
                    .Take(pageSize)                     // Tomar solo 'pageSize' registros
                    .ToListAsync();

                // Crear la respuesta paginada
                var response = new PagedResponse<ProductGroup>(totalCount, pageNumber, pageSize, productsGroup);

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Loguear la excepción (puedes usar tu propia forma de registrar los errores)
                Console.Error.WriteLine($"Error al obtener los grupos de productos: {ex.Message}");

                // Devolver un mensaje de error más detallado
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }


        // GET: api/ProductGroups/5
        /// <summary>
        /// Obtiene un grupo de producto específico utilizando el código del grupo como parámetro de búsqueda.
        /// </summary>
        /// <param name="groupCode">El código único del grupo de productos.</param>
        /// <returns>El grupo de producto solicitado o un error si no se encuentra.</returns>
        [HttpGet("{groupCode}")]
        public async Task<ActionResult<ProductGroup>> GetProductGroup(string groupCode)
        {
            var productGroup = await _context.ProductGroup.FindAsync(groupCode);

            if (productGroup == null)
            {
                return NotFound();
            }

            return productGroup;
        }

        // PUT: api/ProductGroups/5
        /// <summary>
        /// Actualiza los datos de un grupo de producto existente.
        /// </summary>
        /// <param name="groupCode">El código del grupo de producto a actualizar.</param>
        /// <param name="productGroup">Los nuevos datos del grupo de producto.</param>
        /// <returns>Un resultado indicando el éxito o el error de la operación.</returns>
        [HttpPut("{groupCode}")]
        public async Task<IActionResult> PutProductGroup(string groupCode, ProductGroup productGroup)
        {
            if (groupCode != productGroup.ProductGroupCode)
            {
                return BadRequest();
            }

            _context.Entry(productGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductGroupExists(groupCode))
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

        // POST: api/ProductGroups
        /// <summary>
        /// Crea un nuevo grupo de producto en el sistema.
        /// </summary>
        /// <param name="productGroup">El grupo de producto a crear.</param>
        /// <returns>El grupo de producto creado y la ubicación del recurso.</returns>
        [HttpPost]
        public async Task<ActionResult<ProductGroup>> PostProductGroup(ProductGroup productGroup)
        {
            _context.ProductGroup.Add(productGroup);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductGroupExists(productGroup.ProductGroupCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProductGroup", new { groupCode = productGroup.ProductGroupCode }, productGroup);
        }

        // DELETE: api/ProductGroups/5
        /// <summary> 
        /// Elimina un grupo de producto existente.
        /// </summary>
        /// <param name="groupCode">El código del grupo de producto a eliminar.</param>
        /// <returns>Un resultado indicando el éxito o el error de la operación.</returns>
        [HttpDelete("{groupCode}")]
        public async Task<IActionResult> DeleteProductGroup(string groupCode)
        {
            var productGroup = await _context.ProductGroup.FindAsync(groupCode);
            if (productGroup == null)
            {
                return NotFound();
            }

            _context.ProductGroup.Remove(productGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica si un grupo de producto con el código especificado existe en la base de datos.
        /// </summary>
        /// <param name="groupCode">El código del grupo de producto a verificar.</param>
        /// <returns>Verdadero si el grupo de producto existe; falso en caso contrario.</returns>
        private bool ProductGroupExists(string groupCode)
        {
            return _context.ProductGroup.Any(e => e.ProductGroupCode == groupCode);
        }

    }// fin de la clase
}// fin del namespace
