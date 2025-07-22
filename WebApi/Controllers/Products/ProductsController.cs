using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueSelfCheckout.WebApi.Models.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueSelfCheckout.Data;
using AutoMapper;
using BlueSelfCheckout.WebApi.Dtos.Product;
using BlueSelfCheckout.WebApi.Dtos;

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
                var products = await _context.Product
                    .Include(p => p.Material)
                    .Include(p => p.Accompaniment)
                    .ToListAsync();

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
                    .Include(p => p.Material)
                    .Include(p => p.Accompaniment);

                // Si se proporciona un término de búsqueda, filtramos los resultados
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var searchLower = search.ToLower().Trim();
                    query = query.Where(p =>
                        p.ItemName.ToLower().Contains(searchLower) ||
                        (p.EANCode != null && p.EANCode.ToLower().Contains(searchLower)) ||
                        p.ItemCode.ToLower().Contains(searchLower));
                }

                // Calcular el total de productos después de aplicar el filtro
                var totalRecords = await query.CountAsync();

                // Obtener la página de datos solicitada
                var products = await query
                    .OrderBy(p => p.ItemCode)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Mapear la lista de entidades a la lista de DTOs de respuesta
                var productResponseDtos = _mapper.Map<List<ProductDto>>(products);

                // Crear la respuesta paginada
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
                    .Include(p => p.Material)
                    .Include(p => p.Accompaniment)
                    .FirstOrDefaultAsync(p => p.ItemCode == productCode);

                if (product == null)
                {
                    return NotFound($"No se encontró ningún producto con el código: {productCode}.");
                }

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
                    .Include(p => p.Material)
                    .Include(p => p.Accompaniment)
                    .Where(p => p.GroupItemCode == groupCode)
                    .ToListAsync();

                if (!products.Any())
                {
                    return NotFound($"No se encontraron productos para el grupo: {groupCode}.");
                }

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
                    .Include(p => p.Material)
                    .Include(p => p.Accompaniment)
                    .Where(p => p.CategoryItemCode == categoryCode)
                    .ToListAsync();

                if (!products.Any())
                {
                    return NotFound($"No se encontraron productos para la categoría: {categoryCode}.");
                }

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

        // POST: api/Products
        /// <summary>
        /// Crea un nuevo producto, incluyendo sus materiales y acompañamientos.
        /// </summary>
        /// <param name="productCreateDto">Los datos del producto a crear.</param>
        /// <returns>El producto creado y la ubicación del recurso.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductDto productCreateDto)
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

                // Usar transacción para asegurar consistencia
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Crear y guardar el producto principal primero
                    var product = _mapper.Map<Product>(productCreateDto);

                    // Limpiar las colecciones para evitar problemas de mapeo
                    product.Material = new List<ProductMaterial>();
                    product.Accompaniment = new List<ProductAccompaniment>();

                    _context.Product.Add(product);
                    await _context.SaveChangesAsync(); // Guardar primero el producto

                    // 2. Procesar materiales si existen
                    if (productCreateDto.Material != null && productCreateDto.Material.Any())
                    {
                        foreach (var materialDto in productCreateDto.Material)
                        {
                            // Verificar que no exista ya este material para este producto
                            var existingMaterial = await _context.Material
                                .FirstOrDefaultAsync(m => m.ProductItemCode == product.ItemCode && m.ItemCode == materialDto.ItemCode);

                            if (existingMaterial == null)
                            {
                                var material = _mapper.Map<ProductMaterial>(materialDto);
                                material.ProductItemCode = product.ItemCode;
                                _context.Material.Add(material);
                            }
                        }
                    }

                    // 3. Procesar acompañamientos si existen
                    if (productCreateDto.Accompaniment != null && productCreateDto.Accompaniment.Any())
                    {
                        foreach (var accompanimentDto in productCreateDto.Accompaniment)
                        {
                            // Verificar que no exista ya este acompañamiento para este producto
                            var existingAccompaniment = await _context.Accompaniment
                                .FirstOrDefaultAsync(a => a.ProductItemCode == product.ItemCode && a.ItemCode == accompanimentDto.ItemCode);

                            if (existingAccompaniment == null)
                            {
                                var accompaniment = _mapper.Map<ProductAccompaniment>(accompanimentDto);
                                accompaniment.ProductItemCode = product.ItemCode;
                                _context.Accompaniment.Add(accompaniment);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // 4. Recuperar el producto completo para la respuesta
                    var createdProduct = await _context.Product
                        .Include(p => p.Material)
                        .Include(p => p.Accompaniment)
                        .FirstOrDefaultAsync(p => p.ItemCode == product.ItemCode);

                    var productResponseDto = _mapper.Map<ProductDto>(createdProduct);

                    return CreatedAtAction(nameof(GetProduct), new { productCode = productResponseDto.ItemCode }, productResponseDto);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                Console.Error.WriteLine($"Error de base de datos al crear producto: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.Error.WriteLine($"Error interno: {ex.InnerException.Message}");
                }
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
        /// Actualiza un producto existente, incluyendo sus materiales y acompañamientos.
        /// </summary>
        /// <param name="productCode">El código del producto a actualizar.</param>
        /// <param name="productUpdateDto">Los nuevos datos del producto.</param>
        /// <returns>El producto actualizado.</returns>
        [HttpPut("{productCode}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProduct(string productCode, [FromBody] ProductDto productUpdateDto)
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

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Cargar el producto existente y sus colecciones relacionadas
                    var existingProduct = await _context.Product
                        .Include(p => p.Material)
                        .Include(p => p.Accompaniment)
                        .FirstOrDefaultAsync(p => p.ItemCode == productCode);

                    if (existingProduct == null)
                    {
                        return NotFound($"No se encontró un producto con el código: {productCode}.");
                    }

                    // 2. Mapear propiedades escalares del DTO al producto existente
                    _mapper.Map(productUpdateDto, existingProduct);

                    // 3. Manejar la actualización de los materiales
                    await UpdateProductMaterials(existingProduct, productUpdateDto.Material);

                    // 4. Manejar la actualización de los acompañamientos
                    await UpdateProductAccompaniments(existingProduct, productUpdateDto.Accompaniment);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // 5. Recuperar el producto actualizado para la respuesta
                    var updatedProduct = await _context.Product
                        .Include(p => p.Material)
                        .Include(p => p.Accompaniment)
                        .FirstOrDefaultAsync(p => p.ItemCode == productCode);

                    var productResponseDto = _mapper.Map<ProductDto>(updatedProduct);
                    return Ok(productResponseDto);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
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
        /// Elimina un producto existente, incluyendo sus materiales y acompañamientos.
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
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var product = await _context.Product
                        .Include(p => p.Material)
                        .Include(p => p.Accompaniment)
                        .FirstOrDefaultAsync(p => p.ItemCode == productCode);

                    if (product == null)
                    {
                        return NotFound($"No se encontró un producto con el código: {productCode}.");
                    }

                    // Eliminar materiales asociados
                    if (product.Material != null && product.Material.Any())
                    {
                        _context.Material.RemoveRange(product.Material);
                    }

                    // Eliminar acompañamientos asociados
                    if (product.Accompaniment != null && product.Accompaniment.Any())
                    {
                        _context.Accompaniment.RemoveRange(product.Accompaniment);
                    }

                    // Eliminar el producto principal
                    _context.Product.Remove(product);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return NoContent();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
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
        /// Actualiza los materiales de un producto.
        /// </summary>
        /// <param name="existingProduct">El producto existente.</param>
        /// <param name="materialDtos">Los nuevos materiales del DTO.</param>
        private async Task UpdateProductMaterials(Product existingProduct, List<ProductMaterialCreateDto>? materialDtos)
        {
            if (materialDtos != null)
            {
                var existingMaterialItems = existingProduct.Material.ToDictionary(m => m.ItemCode);
                var updatedMaterialItemCodes = new HashSet<string>();

                foreach (var materialDto in materialDtos)
                {
                    if (existingMaterialItems.TryGetValue(materialDto.ItemCode, out var existingMaterial))
                    {
                        // Actualizar material existente
                        _mapper.Map(materialDto, existingMaterial);
                        updatedMaterialItemCodes.Add(materialDto.ItemCode);
                    }
                    else
                    {
                        // Verificar que no exista ya en la base de datos (por si acaso)
                        var dbMaterial = await _context.Material
                            .FirstOrDefaultAsync(m => m.ProductItemCode == existingProduct.ItemCode && m.ItemCode == materialDto.ItemCode);

                        if (dbMaterial == null)
                        {
                            // Nuevo material
                            var newMaterial = _mapper.Map<ProductMaterial>(materialDto);
                            newMaterial.ProductItemCode = existingProduct.ItemCode;
                            existingProduct.Material.Add(newMaterial);
                            updatedMaterialItemCodes.Add(newMaterial.ItemCode);
                        }
                        else
                        {
                            // Ya existe en la base de datos, solo actualizar
                            _mapper.Map(materialDto, dbMaterial);
                            updatedMaterialItemCodes.Add(dbMaterial.ItemCode);
                        }
                    }
                }

                // Eliminar materiales que no están en el DTO de actualización
                var materialsToRemove = existingProduct.Material
                    .Where(m => !updatedMaterialItemCodes.Contains(m.ItemCode))
                    .ToList();

                foreach (var material in materialsToRemove)
                {
                    _context.Material.Remove(material);
                }
            }
        }

        /// <summary>
        /// Actualiza los acompañamientos de un producto.
        /// </summary>
        /// <param name="existingProduct">El producto existente.</param>
        /// <param name="accompanimentDtos">Los nuevos acompañamientos del DTO.</param>
        private async Task UpdateProductAccompaniments(Product existingProduct, List<ProductAccompanimentCreateDto>? accompanimentDtos)
        {
            if (accompanimentDtos != null)
            {
                var existingAccompanimentItems = existingProduct.Accompaniment.ToDictionary(a => a.ItemCode);
                var updatedAccompanimentItemCodes = new HashSet<string>();

                foreach (var accompanimentDto in accompanimentDtos)
                {
                    if (existingAccompanimentItems.TryGetValue(accompanimentDto.ItemCode, out var existingAccompaniment))
                    {
                        // Actualizar acompañamiento existente
                        _mapper.Map(accompanimentDto, existingAccompaniment);
                        updatedAccompanimentItemCodes.Add(accompanimentDto.ItemCode);
                    }
                    else
                    {
                        // Verificar que no exista ya en la base de datos (por si acaso)
                        var dbAccompaniment = await _context.Accompaniment
                            .FirstOrDefaultAsync(a => a.ProductItemCode == existingProduct.ItemCode && a.ItemCode == accompanimentDto.ItemCode);

                        if (dbAccompaniment == null)
                        {
                            // Nuevo acompañamiento
                            var newAccompaniment = _mapper.Map<ProductAccompaniment>(accompanimentDto);
                            newAccompaniment.ProductItemCode = existingProduct.ItemCode;
                            existingProduct.Accompaniment.Add(newAccompaniment);
                            updatedAccompanimentItemCodes.Add(newAccompaniment.ItemCode);
                        }
                        else
                        {
                            // Ya existe en la base de datos, solo actualizar
                            _mapper.Map(accompanimentDto, dbAccompaniment);
                            updatedAccompanimentItemCodes.Add(dbAccompaniment.ItemCode);
                        }
                    }
                }

                // Eliminar acompañamientos que no están en el DTO de actualización
                var accompanimentsToRemove = existingProduct.Accompaniment
                    .Where(a => !updatedAccompanimentItemCodes.Contains(a.ItemCode))
                    .ToList();

                foreach (var accompaniment in accompanimentsToRemove)
                {
                    _context.Accompaniment.Remove(accompaniment);
                }
            }
        }

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