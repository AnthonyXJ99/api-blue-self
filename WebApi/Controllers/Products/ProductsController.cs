using AutoMapper;
using BlueSelfCheckout.Data;
using BlueSelfCheckout.Models;
using BlueSelfCheckout.WebApi.Dtos;
using BlueSelfCheckout.WebApi.Dtos.Product;
using BlueSelfCheckout.WebApi.Models.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueSelfCheckout.WebApi.Controllers.Products
{
    /// <summary>
    /// Controlador que gestiona los productos en el sistema.
    /// Permite obtener, crear, actualizar y eliminar productos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia del controlador <see cref="ProductsController"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>
        /// <param name="mapper">El mapeador de objetos AutoMapper.</param>
        public ProductsController(ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
        /// Procesa las URLs de imágenes de un producto y sus ingredientes
        /// </summary>
        /// <param name="product">Producto a procesar</param>
        private void ProcessImageUrls(Product product)
        {
            // Procesar URL de imagen del producto
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                product.ImageUrl = BuildPublicUrl(product.ImageUrl);
            }

            // Procesar URLs de imágenes de ingredientes
            if (product.ProductTree?.Items1 != null)
            {
                foreach (var ingredient in product.ProductTree.Items1)
                {
                    if (!string.IsNullOrEmpty(ingredient.ImageUrl))
                    {
                        ingredient.ImageUrl = BuildPublicUrl(ingredient.ImageUrl);
                    }
                }
            }
        }

        // GET: api/Products/all
        /// <summary>
        /// Obtiene una lista de todos los productos sin paginación ni filtros.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="ProductDto"/>.</returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            try
            {
                // Cargar productos con ProductTree y acompañamientos
                var products = await _context.Product
                    .Include(p => p.ProductTree)
                        .ThenInclude(pt => pt.Items1)
                    .Include(p => p.Accompaniment)
                    .Where(p=>p.SellItem!="N")
                    .ToListAsync();

                // Procesar URLs de imágenes
                foreach (var product in products)
                {
                    ProcessImageUrls(product);
                }

                // AutoMapper mapea automáticamente ProductTree.Items1 a ProductDto.Material
                var productDtos = _mapper.Map<List<ProductDto>>(products);
                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al obtener todos los productos: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }

        // GET: api/Products
        /// <summary>
        /// Obtiene una lista paginada de productos con capacidad de búsqueda.
        /// </summary>
        /// <param name="pageNumber">Número de página (por defecto: 1).</param>
        /// <param name="pageSize">Tamaño de página (por defecto: 10).</param>
        /// <param name="search">Término de búsqueda para ItemName, EANCode o ItemCode.</param>
        /// <returns>Una respuesta paginada de objetos <see cref="ProductDto"/>.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResult<ProductDto>>> GetProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            try
            {
                // Validar parámetros de paginación
                if (pageNumber < 1)
                {
                    return BadRequest("El número de página debe ser mayor que 0.");
                }
                if (pageSize < 1 || pageSize > 100)
                {
                    return BadRequest("El tamaño de página debe estar entre 1 y 100.");
                }

                // Comenzamos la consulta con todos los productos
                IQueryable<Product> query = _context.Product
                    .Include(p => p.ProductTree)
                        .ThenInclude(pt => pt.Items1)
                    .Include(p => p.Accompaniment);

                // Si se proporciona un término de búsqueda, filtramos los resultados
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var searchLower = search.ToLower().Trim();
                    query = query.Where(p =>
                        p.ItemName.ToLower().Contains(searchLower) ||
                        (p.EANCode != null && p.EANCode.ToLower().Contains(searchLower)) ||
                        p.ItemCode.ToLower().Contains(searchLower) ||
                        (p.FrgnName != null && p.FrgnName.ToLower().Contains(searchLower)));
                }

                // Calcular el total de productos después de aplicar el filtro
                var totalRecords = await query.CountAsync();

                // Obtener la página de datos solicitada
                var products = await query
                    .OrderBy(p => p.ItemCode)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Procesar URLs de imágenes
                foreach (var product in products)
                {
                    ProcessImageUrls(product);
                }

                // AutoMapper mapea automáticamente ProductTree.Items1 a ProductDto.Material
                var productResponseDtos = _mapper.Map<List<ProductDto>>(products);

                // Mantener la estructura PaginatedResult original
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                var response = new PaginatedResult<ProductDto>
                {
                    Data = productResponseDtos,
                    Page = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    HasNextPage = pageNumber < totalPages,
                    HasPreviousPage = pageNumber > 1,
                    Filter = search
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al obtener los productos: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una lista de todos los productos que son vendibles (SellItem = 'Y').
        /// </summary>
        /// <returns>Una lista de objetos <see cref="ProductDto"/> correspondientes a productos vendibles.</returns>
        [HttpGet("sellable/{sellable}")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetSellableProducts(Boolean sellable)
        {
            try
            {
                var sellItemValue = sellable ? "Y" : "N";
                var products = await _context.Product
                    .Include(p => p.ProductTree)
                        .ThenInclude(pt => pt.Items1)
                    .Include(p => p.Accompaniment)
                    .Where(p => p.SellItem == sellItemValue)
                    .ToListAsync();

                // Procesar URLs de imágenes
                foreach (var product in products)
                {
                    ProcessImageUrls(product);
                }

                // AutoMapper mapea automáticamente ProductTree.Items1 a ProductDto.Material
                var productDtos = _mapper.Map<List<ProductDto>>(products);
                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al obtener productos vendibles: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }

        // GET: api/Products/{productCode}
        /// <summary>
        /// Obtiene un producto específico utilizando el código del producto.
        /// </summary>
        /// <param name="productCode">El código único del producto.</param>
        /// <returns>El producto solicitado o un error si no se encuentra.</returns>
        [HttpGet("{productCode}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> GetProduct(string productCode)
        {
            try
            {
                var product = await _context.Product
                    .Include(p => p.ProductTree)
                        .ThenInclude(pt => pt.Items1)
                    .Include(p => p.Accompaniment)
                    .FirstOrDefaultAsync(p => p.ItemCode == productCode);

                if (product == null)
                {
                    return NotFound($"No se encontró ningún producto con el código: {productCode}.");
                }

                // Procesar URLs de imágenes
                ProcessImageUrls(product);

                // AutoMapper mapea automáticamente ProductTree.Items1 a ProductDto.Material
                var productResponseDto = _mapper.Map<ProductDto>(product);
                return Ok(productResponseDto);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al obtener el producto con código {productCode}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }

        // GET: api/Products/Groups/{groupCode}
        /// <summary>
        /// Obtiene una lista de productos que pertenecen a un grupo de productos específico.
        /// </summary>
        /// <param name="groupCode">El código del grupo de productos.</param>
        /// <returns>Una lista de productos con el grupo especificado.</returns>
        [HttpGet("Groups/{groupCode}")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByGroup(string groupCode)
        {
            try
            {
                var products = await _context.Product
                    .Include(p => p.ProductTree)
                        .ThenInclude(pt => pt.Items1)
                    .Include(p => p.Accompaniment)
                    .Where(p => p.GroupItemCode == groupCode)
                    .ToListAsync();

                if (!products.Any())
                {
                    return NotFound($"No se encontraron productos para el grupo: {groupCode}.");
                }

                // Procesar URLs de imágenes
                foreach (var product in products)
                {
                    ProcessImageUrls(product);
                }

                // AutoMapper mapea automáticamente ProductTree.Items1 a ProductDto.Material
                var productDtos = _mapper.Map<List<ProductDto>>(products);
                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al obtener productos por grupo {groupCode}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }

        // GET: api/Products/Categories/{categoryCode}
        /// <summary>
        /// Obtiene una lista de productos que pertenecen a una categoría de productos específica.
        /// </summary>
        /// <param name="categoryCode">El código de la categoría de productos.</param>
        /// <returns>Una lista de productos con la categoría especificada.</returns>
        [HttpGet("Categories/{categoryCode}")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(string categoryCode)
        {
            try
            {
                var products = await _context.Product
                    .Include(p => p.ProductTree)
                        .ThenInclude(pt => pt.Items1)
                    .Include(p => p.Accompaniment)
                    .Where(p => p.CategoryItemCode == categoryCode)
                    .ToListAsync();

                if (!products.Any())
                {
                    return NotFound($"No se encontraron productos para la categoría: {categoryCode}.");
                }

                // Procesar URLs de imágenes
                foreach (var product in products)
                {
                    ProcessImageUrls(product);
                }

                // AutoMapper mapea automáticamente ProductTree.Items1 a ProductDto.Material
                var productDtos = _mapper.Map<List<ProductDto>>(products);
                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al obtener productos por categoría {categoryCode}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }

        // GET: api/Products/available
        /// <summary>
        /// Obtiene solo los productos disponibles (Available = 'Y' y Enabled = 'Y').
        /// </summary>
        /// <returns>Lista de productos disponibles con URLs completas.</returns>
        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAvailableProducts()
        {
            try
            {
                var products = await _context.Product
                    .Include(p => p.ProductTree)
                        .ThenInclude(pt => pt.Items1)
                    .Include(p => p.Accompaniment)
                    .Where(p => p.Available == "Y" && p.Enabled == "Y" && p.SellItem == "Y")
                    .OrderBy(p => p.ItemName)
                    .ToListAsync();

                // Procesar URLs de imágenes
                foreach (var product in products)
                {
                    ProcessImageUrls(product);
                }

                // AutoMapper mapea automáticamente ProductTree.Items1 a ProductDto.Material
                var productDtos = _mapper.Map<List<ProductDto>>(products);
                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al obtener productos disponibles: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }

        // POST: api/Products
        /// <summary>
        /// Crea un nuevo producto (solo datos principales, sin materiales ni acompañamientos).
        /// </summary>
        /// <param name="productCreateDto">Los datos del producto a crear.</param>
        /// <returns>El producto creado y la ubicación del recurso.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductCreateDto productCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar si el ItemCode ya existe
                if (await _context.Product.AnyAsync(p => p.ItemCode == productCreateDto.ItemCode))
                {
                    return Conflict($"Ya existe un producto con el ItemCode: {productCreateDto.ItemCode}.");
                }

                // Mapear DTO a entidad
                var product = _mapper.Map<Product>(productCreateDto);

                // Agregar el producto a la base de datos
                _context.Product.Add(product);
                await _context.SaveChangesAsync();

                // Recuperar el producto creado con sus relaciones para la respuesta
                var createdProduct = await _context.Product
                    .Include(p => p.ProductTree)
                        .ThenInclude(pt => pt.Items1)
                    .Include(p => p.Accompaniment)
                    .FirstOrDefaultAsync(p => p.ItemCode == product.ItemCode);

                // Procesar URLs de imágenes
                ProcessImageUrls(createdProduct);

                // Mapear a DTO de respuesta
                var productResponseDto = _mapper.Map<ProductDto>(createdProduct);

                return CreatedAtAction(nameof(GetProduct), new { productCode = productResponseDto.ItemCode }, productResponseDto);
            }
            catch (DbUpdateException ex)
            {
                Console.Error.WriteLine($"Error de base de datos al crear producto: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error de base de datos al crear el producto", error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al crear producto: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error interno del servidor al crear el producto", error = ex.Message });
            }
        }

        // PUT: api/Products/{productCode}
        /// <summary>
        /// Actualiza un producto existente (solo datos principales, sin materiales ni acompañamientos).
        /// </summary>
        /// <param name="productCode">El código del producto a actualizar.</param>
        /// <param name="productUpdateDto">Los nuevos datos del producto.</param>
        /// <returns>El producto actualizado.</returns>
        [HttpPut("{productCode}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProduct(string productCode, [FromBody] ProductUpdateDto productUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar consistencia entre parámetro de ruta y DTO
                if (productCode != productUpdateDto.ItemCode)
                {
                    return BadRequest("El código del producto en la URL no coincide con el del cuerpo de la solicitud.");
                }

                // Buscar el producto existente
                var existingProduct = await _context.Product.FirstOrDefaultAsync(p => p.ItemCode == productCode);

                if (existingProduct == null)
                {
                    return NotFound($"No se encontró un producto con el código: {productCode}.");
                }

                // Mapear los cambios del DTO al producto existente
                _mapper.Map(productUpdateDto, existingProduct);

                // Guardar los cambios
                await _context.SaveChangesAsync();

                // Recuperar el producto actualizado con sus relaciones para la respuesta
                var updatedProduct = await _context.Product
                    .Include(p => p.ProductTree)
                        .ThenInclude(pt => pt.Items1)
                    .Include(p => p.Accompaniment)
                    .FirstOrDefaultAsync(p => p.ItemCode == productCode);

                // Procesar URLs de imágenes
                ProcessImageUrls(updatedProduct);

                // Mapear a DTO de respuesta
                var productResponseDto = _mapper.Map<ProductDto>(updatedProduct);
                return Ok(productResponseDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProductExistsAsync(productCode))
                {
                    return NotFound($"No se encontró un producto con el código: {productCode} para actualizar.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al actualizar el producto con código {productCode}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error interno del servidor al actualizar el producto", error = ex.Message });
            }
        }

        // DELETE: api/Products/{productCode}
        /// <summary>
        /// Elimina un producto existente (solo el producto principal).
        /// Los materiales y acompañamientos se gestionan desde sus respectivos controladores.
        /// </summary>
        /// <param name="productCode">El código del producto a eliminar.</param>
        /// <returns>Un resultado indicando el éxito o el error de la operación.</returns>
        [HttpDelete("{productCode}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProduct(string productCode)
        {
            try
            {
                var product = await _context.Product.FirstOrDefaultAsync(p => p.ItemCode == productCode);

                if (product == null)
                {
                    return NotFound($"No se encontró un producto con el código: {productCode}.");
                }

                // Eliminar solo el producto principal
                // Los materiales y acompañamientos se eliminan por cascada o desde sus controladores
                _context.Product.Remove(product);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al eliminar el producto con código {productCode}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error interno del servidor al eliminar el producto", error = ex.Message });
            }
        }

        #region Métodos Privados de Ayuda

        /// <summary>
        /// Verifica si un producto con el código especificado existe en la base de datos.
        /// </summary>
        /// <param name="productCode">El código del producto a verificar.</param>
        /// <returns>Verdadero si el producto existe; falso en caso contrario.</returns>
        private async Task<bool> ProductExistsAsync(string productCode)
        {
            return await _context.Product.AnyAsync(e => e.ItemCode == productCode);
        }

        #endregion
    }
}