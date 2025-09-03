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
        /// Construye la URL pública completa a partir de una ruta relativa.
        /// </summary>
        /// <param name="relativePath">Ruta relativa (ej: /images/archivo.jpg)</param>
        /// <returns>URL completa</returns>
        private string BuildPublicUrl(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            return $"{baseUrl}{relativePath}";
        }

        /// <summary>
        /// Procesa un grupo para incluir URLs completas de imágenes manteniendo compatibilidad.
        /// </summary>
        /// <param name="group">Grupo a procesar</param>
        /// <returns>ProductGroup con ImageUrl procesada</returns>
        private ProductGroup ProcessGroupWithImages(ProductGroup group)
        {
            // ✅ Crear una copia del grupo original
            var processedGroup = new ProductGroup
            {
                ProductGroupCode = group.ProductGroupCode,
                ProductGroupName = group.ProductGroupName,
                FrgnName = group.FrgnName,
                // ✅ CAMBIO CLAVE: ImageUrl ahora contiene la URL completa
                ImageUrl = !string.IsNullOrEmpty(group.ImageUrl) ? BuildPublicUrl(group.ImageUrl) : group.ImageUrl,
                Description = group.Description,
                FrgnDescription = group.FrgnDescription,
                Enabled = group.Enabled,
                VisOrder = group.VisOrder,
                DataSource = group.DataSource,
                ProductGroupCodeERP = group.ProductGroupCodeERP,
                ProductGroupCodePOS = group.ProductGroupCodePOS
            };

            return processedGroup;
        }

        /// <summary>
        /// Procesa una lista de grupos para incluir URLs completas de imágenes.
        /// </summary>
        /// <param name="groups">Lista de grupos a procesar</param>
        /// <returns>Lista de ProductGroup con URLs construidas dinámicamente</returns>
        private IEnumerable<ProductGroup> ProcessGroupsWithImages(IEnumerable<ProductGroup> groups)
        {
            return groups.Select(ProcessGroupWithImages);
        }

        /// <summary>
        /// Obtiene una lista de todos los grupos de productos.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="ProductGroup"/> con URLs completas.</returns>
        // GET: api/ProductGroups/all 
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ProductGroup>>> GetAllProductGroup()
        {
            var groups = await _context.ProductGroup.ToListAsync();
            var processedGroups = ProcessGroupsWithImages(groups);
            return Ok(processedGroups);
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
                    // Realizamos una búsqueda que sea insensible a mayúsculas y minúsculas en el código y nombre del grupo de producto
                    query = query.Where(pg => pg.ProductGroupCode.Contains(search) ||
                                             pg.ProductGroupName.Contains(search) ||
                                             (pg.FrgnName != null && pg.FrgnName.Contains(search)));
                }

                // Calcular el total de grupo de productos después de aplicar el filtro
                var totalCount = await query.CountAsync();

                // Obtener la página de datos solicitada
                var productsGroup = await query
                    .OrderBy(pg => pg.VisOrder) // Ordenar por VisOrder
                    .ThenBy(pg => pg.ProductGroupName) // Luego por nombre
                    .Skip((pageNumber - 1) * pageSize)  // Saltar los primeros (pageNumber - 1) * pageSize registros
                    .Take(pageSize)                     // Tomar solo 'pageSize' registros
                    .ToListAsync();

                // ✅ Procesar grupos con URLs dinámicas manteniendo compatibilidad
                var processedGroups = ProcessGroupsWithImages(productsGroup);

                // ✅ MANTENER la estructura PagedResponse original
                var response = new PagedResponse<ProductGroup>(totalCount, pageNumber, pageSize, processedGroups.ToList());

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

            // ✅ Procesar grupo con URL dinámica manteniendo compatibilidad
            var processedGroup = ProcessGroupWithImages(productGroup);
            return Ok(processedGroup);
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

            // ✅ Procesar grupo creado con URL dinámica manteniendo compatibilidad
            var processedGroup = ProcessGroupWithImages(productGroup);
            return CreatedAtAction("GetProductGroup", new { groupCode = productGroup.ProductGroupCode }, processedGroup);
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

        // GET: api/ProductGroups/enabled
        /// <summary>
        /// Obtiene solo los grupos habilitados (Enabled = 'Y').
        /// </summary>
        /// <returns>Lista de grupos habilitados con URLs completas.</returns>
        [HttpGet("enabled")]
        public async Task<ActionResult<IEnumerable<ProductGroup>>> GetEnabledGroups()
        {
            var enabledGroups = await _context.ProductGroup
                .Where(pg => pg.Enabled == "Y")
                .OrderBy(pg => pg.VisOrder)
                .ThenBy(pg => pg.ProductGroupName)
                .ToListAsync();

            var processedGroups = ProcessGroupsWithImages(enabledGroups);
            return Ok(processedGroups);
        }

        // GET: api/ProductGroups/with-images
        /// <summary>
        /// Obtiene solo los grupos que tienen imagen asignada.
        /// </summary>
        /// <returns>Lista de grupos con imagen y URLs completas.</returns>
        [HttpGet("with-images")]
        public async Task<ActionResult<IEnumerable<ProductGroup>>> GetGroupsWithImages()
        {
            var groupsWithImages = await _context.ProductGroup
                .Where(pg => !string.IsNullOrEmpty(pg.ImageUrl))
                .OrderBy(pg => pg.VisOrder)
                .ThenBy(pg => pg.ProductGroupName)
                .ToListAsync();

            var processedGroups = ProcessGroupsWithImages(groupsWithImages);
            return Ok(processedGroups);
        }

        // GET: api/ProductGroups/{groupCode}/categories
        /// <summary>
        /// Obtiene las categorías que pertenecen a un grupo específico.
        /// </summary>
        /// <param name="groupCode">Código del grupo</param>
        /// <returns>Lista de categorías del grupo</returns>
        [HttpGet("{groupCode}/categories")]
        public async Task<ActionResult<IEnumerable<object>>> GetCategoriesByGroup(string groupCode)
        {
            // Verificar que el grupo existe
            var groupExists = await _context.ProductGroup.AnyAsync(pg => pg.ProductGroupCode == groupCode);
            if (!groupExists)
            {
                return NotFound("Grupo de productos no encontrado");
            }

            var categories = await _context.ProductCategory
                .Where(pc => pc.GroupItemCode == groupCode)
                .OrderBy(pc => pc.VisOrder)
                .ThenBy(pc => pc.CategoryItemName)
                .ToListAsync();

            // Procesar categorías con URLs dinámicas (si tienen imagen)
            var processedCategories = categories.Select(cat => new
            {
                cat.CategoryItemCode,
                cat.CategoryItemName,
                cat.FrgnName,
                ImageUrl = !string.IsNullOrEmpty(cat.ImageUrl) ? BuildPublicUrl(cat.ImageUrl) : null,
                ImageRelativePath = cat.ImageUrl,
                cat.Description,
                cat.FrgnDescription,
                cat.VisOrder,
                cat.Enabled,
                cat.DataSource,
                cat.GroupItemCode
            });

            return Ok(processedCategories);
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