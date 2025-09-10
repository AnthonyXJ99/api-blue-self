using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueSelfCheckout.WebApi.Models.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueSelfCheckout.Data;
using BlueSelfCheckout.WebApi.Models;

namespace BlueSelfCheckout.WebApi.Controllers.Products
{
    /// <summary>
    /// Controlador que gestiona las categorías de productos en el sistema.
    /// Permite obtener, crear, actualizar y eliminar categorías de productos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Inicializa una nueva instancia del controlador <see cref="ProductCategoriesController"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>
        public ProductCategoriesController(ApplicationDBContext context)
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
        /// Procesa una categoría para incluir URLs completas de imágenes manteniendo compatibilidad.
        /// </summary>
        /// <param name="category">Categoría a procesar</param>
        /// <returns>ProductCategory con ImageUrl procesada</returns>
        private ProductCategory ProcessCategoryWithImages(ProductCategory category)
        {
            // ✅ Crear una copia de la categoría original
            var processedCategory = new ProductCategory
            {
                CategoryItemCode = category.CategoryItemCode,
                CategoryItemName = category.CategoryItemName,
                FrgnName = category.FrgnName,
                // ✅ CAMBIO CLAVE: ImageUrl ahora contiene la URL completa
                ImageUrl = !string.IsNullOrEmpty(category.ImageUrl) ? BuildPublicUrl(category.ImageUrl) : category.ImageUrl,
                Description = category.Description,
                FrgnDescription = category.FrgnDescription,
                VisOrder = category.VisOrder,
                Enabled = category.Enabled,
                DataSource = category.DataSource,
                GroupItemCode = category.GroupItemCode
            };

            return processedCategory;
        }

        /// <summary>
        /// Procesa una lista de categorías para incluir URLs completas de imágenes.
        /// </summary>
        /// <param name="categories">Lista de categorías a procesar</param>
        /// <returns>Lista de ProductCategory con URLs construidas dinámicamente</returns>
        private IEnumerable<ProductCategory> ProcessCategoriesWithImages(IEnumerable<ProductCategory> categories)
        {
            return categories.Select(ProcessCategoryWithImages);
        }

        // GET: api/ProductCategories/all
        /// <summary>
        /// Obtiene una lista de todas las categorías de productos.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="ProductCategory"/> con URLs completas.</returns>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetAllProductCategory()
        {
            var categories = await _context.ProductCategory.ToListAsync();
            var processedCategories = ProcessCategoriesWithImages(categories);
            return Ok(processedCategories);
        }

        // GET: api/ProductCategories
        [HttpGet]
        public async Task<ActionResult<PagedResponse<ProductCategory>>> GetProductCategory(
            int pageNumber = 1,
            int pageSize = 10,
            string search = null) // Parámetro de búsqueda (search)
        {
            try
            {
                // Comenzamos la consulta con todas las categorias de productos
                IQueryable<ProductCategory> query = _context.ProductCategory;

                // Si se proporciona un término de búsqueda, filtramos los resultados
                if (!string.IsNullOrEmpty(search))
                {
                    // Realizamos una búsqueda que sea insensible a mayúsculas y minúsculas en el código y nombre de la categoría de producto
                    query = query.Where(pc => pc.CategoryItemCode.Contains(search) ||
                                             pc.CategoryItemName.Contains(search) ||
                                             (pc.FrgnName != null && pc.FrgnName.Contains(search)));
                }

                // Calcular el total de categoría de productos después de aplicar el filtro
                var totalCount = await query.CountAsync();

                // Obtener la página de datos solicitada
                var productsCategory = await query
                    .OrderBy(pc => pc.VisOrder) // Ordenar por VisOrder
                    .ThenBy(pc => pc.CategoryItemName) // Luego por nombre
                    .Skip((pageNumber - 1) * pageSize)  // Saltar los primeros (pageNumber - 1) * pageSize registros
                    .Take(pageSize)                     // Tomar solo 'pageSize' registros
                    .ToListAsync();

                // ✅ Procesar categorías con URLs dinámicas manteniendo compatibilidad
                var processedCategories = ProcessCategoriesWithImages(productsCategory);

                // ✅ MANTENER la estructura PagedResponse original
                var response = new PagedResponse<ProductCategory>(totalCount, pageNumber, pageSize, processedCategories.ToList());

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Loguear la excepción (puedes usar tu propia forma de registrar los errores)
                Console.Error.WriteLine($"Error al obtener las Categorias de productos: {ex.Message}");

                // Devolver un mensaje de error más detallado
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }

        // GET: api/ProductCategories/5
        /// <summary>
        /// Obtiene una categoría de producto específica utilizando el código de la categoría como parámetro de búsqueda.
        /// </summary>
        /// <param name="categoryCode">El código único de la categoría de producto.</param>
        /// <returns>La categoría de producto solicitada o un error si no se encuentra.</returns>
        [HttpGet("{categoryCode}")]
        public async Task<ActionResult<ProductCategory>> GetProductCategory(string categoryCode)
        {
            var productCategory = await _context.ProductCategory.FindAsync(categoryCode);

            if (productCategory == null)
            {
                return NotFound();
            }

            // ✅ Procesar categoría con URL dinámica manteniendo compatibilidad
            var processedCategory = ProcessCategoryWithImages(productCategory);
            return Ok(processedCategory);
        }

        // PUT: api/ProductCategories/5
        /// <summary>
        /// Actualiza los datos de una categoría de producto existente.
        /// </summary>
        /// <param name="categoryCode">El código de la categoría de producto a actualizar.</param>
        /// <param name="productCategory">Los nuevos datos de la categoría de producto.</param>
        /// <returns>Un resultado indicando el éxito o el error de la operación.</returns>
        [HttpPut("{categoryCode}")]
        public async Task<IActionResult> PutProductCategory(string categoryCode, ProductCategory productCategory)
        {
            if (categoryCode != productCategory.CategoryItemCode)
            {
                return BadRequest();
            }

            _context.Entry(productCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductCategoryExists(categoryCode))
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

        // POST: api/ProductCategories
        /// <summary>
        /// Crea una nueva categoría de producto en el sistema.
        /// </summary>
        /// <param name="productCategory">La categoría de producto a crear.</param>
        /// <returns>La categoría de producto creada y la ubicación del recurso.</returns>
        [HttpPost]
        public async Task<ActionResult<ProductCategory>> PostProductCategory(ProductCategory productCategory)
        {
            _context.ProductCategory.Add(productCategory);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductCategoryExists(productCategory.CategoryItemCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // ✅ Procesar categoría creada con URL dinámica manteniendo compatibilidad
            var processedCategory = ProcessCategoryWithImages(productCategory);
            return CreatedAtAction("GetProductCategory", new { categoryCode = productCategory.CategoryItemCode }, processedCategory);
        }

        // DELETE: api/ProductCategories/5
        /// <summary>
        /// Elimina una categoría de producto existente.
        /// </summary>
        /// <param name="categoryCode">El código de la categoría de producto a eliminar.</param>
        /// <returns>Un resultado indicando el éxito o el error de la operación.</returns>
        [HttpDelete("{categoryCode}")]
        public async Task<IActionResult> DeleteProductCategory(string categoryCode)
        {
            var productCategory = await _context.ProductCategory.FindAsync(categoryCode);
            if (productCategory == null)
            {
                return NotFound();
            }

            _context.ProductCategory.Remove(productCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/ProductCategories/Groups/{groupCode}
        /// <summary>
        /// Obtiene una lista de categorias que pertenecen a un grupo de productos específico.
        /// </summary>
        /// <param name="groupCode">El código del grupo de productos.</param>
        /// <returns>Una lista de categorías con el grupo especificado.</returns>
        [HttpGet("Groups/{groupCode}")]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetCategoriesByGroup(string groupCode)
        {
            var productCategories = await _context.ProductCategory
                                          .Where(p => p.GroupItemCode == groupCode)
                                          .OrderBy(p => p.VisOrder)
                                          .ThenBy(p => p.CategoryItemName)
                                          .ToListAsync();

            if (productCategories == null || !productCategories.Any())
            {
                return NotFound(); // Retorna 404 si no se encuentran productos para el grupo
            }

            // ✅ Procesar categorías con URLs dinámicas manteniendo compatibilidad
            var processedCategories = ProcessCategoriesWithImages(productCategories);
            return Ok(processedCategories);
        }

        // GET: api/ProductCategories/enabled
        /// <summary>
        /// Obtiene solo las categorías habilitadas (Enabled = 'Y').
        /// </summary>
        /// <returns>Lista de categorías habilitadas con URLs completas.</returns>
        [HttpGet("enabled")]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetEnabledCategories()
        {
            var enabledCategories = await _context.ProductCategory
                .Where(pc => pc.Enabled == "Y")
                .OrderBy(pc => pc.VisOrder)
                .ThenBy(pc => pc.CategoryItemName)
                .ToListAsync();

            var processedCategories = ProcessCategoriesWithImages(enabledCategories);
            return Ok(processedCategories);
        }

        // GET: api/ProductCategories/with-images
        /// <summary>
        /// Obtiene solo las categorías que tienen imagen asignada.
        /// </summary>
        /// <returns>Lista de categorías con imagen y URLs completas.</returns>
        [HttpGet("with-images")]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetCategoriesWithImages()
        {
            var categoriesWithImages = await _context.ProductCategory
                .Where(pc => !string.IsNullOrEmpty(pc.ImageUrl))
                .OrderBy(pc => pc.VisOrder)
                .ThenBy(pc => pc.CategoryItemName)
                .ToListAsync();

            var processedCategories = ProcessCategoriesWithImages(categoriesWithImages);
            return Ok(processedCategories);
        }


        // GET: api/ProductCategories/with-accompaniments
        /// <summary>
        /// Obtiene las categorías habilitadas que tienen acompañamientos configurados, incluyendo los detalles de los acompañamientos.
        /// </summary>
        /// <returns>Lista de categorías con sus acompañamientos y URLs completas.</returns>
        [HttpGet("with-accompaniments")]
        public async Task<ActionResult<IEnumerable<object>>> GetCategoriesWithAccompaniments()
        {
            try
            {
                var categoriesWithAccompaniments = await _context.ProductCategory
                    .Include(c => c.Accompaniments)
                        .ThenInclude(a => a.AccompanimentProduct)
                    .Where(c => c.Enabled == "Y")
                    .OrderBy(c => c.VisOrder)
                    .ThenBy(c => c.CategoryItemName)
                    .ToListAsync();

                var result = categoriesWithAccompaniments.Select(category => new
                {
                    CategoryItemCode = category.CategoryItemCode,
                    CategoryItemName = category.CategoryItemName,
                    FrgnName = category.FrgnName,
                    ImageUrl = BuildPublicUrl(category.ImageUrl),
                    Description = category.Description,
                    FrgnDescription = category.FrgnDescription,
                    VisOrder = category.VisOrder,
                    Enabled = category.Enabled,
                    DataSource = category.DataSource,
                    GroupItemCode = category.GroupItemCode,
                    Accompaniments = category.Accompaniments.Select(acc => new
                    {
                        LineNumber = acc.LineNumber,
                        AccompanimentItemCode = acc.AccompanimentItemCode,
                        AccompanimentItemName = acc.AccompanimentProduct?.ItemName ?? string.Empty,
                        AccompanimentImageUrl = BuildPublicUrl(acc.AccompanimentProduct?.ImageUrl),
                        AccompanimentPrice = acc.AccompanimentProduct?.Price ?? 0,
                        Discount = acc.Discount,
                        EnlargementItemCode = acc.EnlargementItemCode,
                        EnlargementDiscount = acc.EnlargementDiscount
                    }).OrderBy(a => a.LineNumber).ToList()
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }


        /// <summary>
        /// Verifica si una categoría de producto con el código especificado existe en la base de datos.
        /// </summary>
        /// <param name="categoryCode">El código de la categoría de producto a verificar.</param>
        /// <returns>Verdadero si la categoría de producto existe; falso en caso contrario.</returns>
        private bool ProductCategoryExists(string categoryCode)
        {
            return _context.ProductCategory.Any(e => e.CategoryItemCode == categoryCode);
        }




    }// fin de la clase




}// fin del namespace