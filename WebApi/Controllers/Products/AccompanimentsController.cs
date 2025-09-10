using AutoMapper;
using BlueSelfCheckout.Data;
using BlueSelfCheckout.WebApi.Dtos.Product;
using BlueSelfCheckout.WebApi.Models.Admin;
using BlueSelfCheckout.WebApi.Models.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BlueSelfCheckout.WebApi.Controllers.Products
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccompanimentsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;

        public AccompanimentsController(ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene todas las categorías con sus acompañamientos
        /// </summary>
        /// <returns>Lista de categorías con acompañamientos</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryWithAccompanimentsDTO>>> GetCategoriesWithAccompaniments()
        {
            try
            {
                var categories = await _context.ProductCategory
                .Include(c => c.Accompaniments)
                    .ThenInclude(a => a.AccompanimentProduct)
                .Where(c => c.Enabled == "Y" && c.Accompaniments.Any()) // ← Aquí está la clave
                .OrderBy(c => c.VisOrder)
                .ToListAsync();



                var result = _mapper.Map<IEnumerable<CategoryWithAccompanimentsDTO>>(categories);
                foreach (var category in result)
                {
            
                    ProcessCategoryImageUrls(category);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene los acompañamientos de una categoría específica
        /// </summary>
        /// <param name="categoryItemCode">Código de la categoría</param>
        /// <returns>Categoría con sus acompañamientos</returns>
        [HttpGet("{categoryItemCode}")]
        public async Task<ActionResult<CategoryWithAccompanimentsDTO>> GetCategoryAccompaniments(string categoryItemCode)
        {
            try
            {
                var category = await _context.ProductCategory
                    .Include(c => c.Accompaniments)
                        .ThenInclude(a => a.AccompanimentProduct)
                    .FirstOrDefaultAsync(c => c.CategoryItemCode == categoryItemCode);

                if (category == null)
                {
                    return NotFound($"Categoría con código '{categoryItemCode}' no encontrada");
                }

                var result = _mapper.Map<CategoryWithAccompanimentsDTO>(category);
                foreach (var accompaniment in result.AvailableAccompaniments)
                {
                    ProcessAccompanimentImageUrls(accompaniment);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene un acompañamiento específico de una categoría
        /// </summary>
        /// <param name="categoryItemCode">Código de la categoría</param>
        /// <param name="lineNumber">Número de línea del acompañamiento</param>
        /// <returns>Acompañamiento específico</returns>
        [HttpGet("{categoryItemCode}/{lineNumber:int}")]
        public async Task<ActionResult<CategoryAccompanimentDTO>> GetAccompaniment(string categoryItemCode, int lineNumber)
        {
            try
            {
                var accompaniment = await _context.CategoryAccompaniments
                    .Include(a => a.AccompanimentProduct)
                    .Include(a => a.Category)
                    .FirstOrDefaultAsync(a => a.CategoryItemCode == categoryItemCode && a.LineNumber == lineNumber);

                if (accompaniment == null)
                {
                    return NotFound($"Acompañamiento no encontrado para categoría '{categoryItemCode}' y línea {lineNumber}");
                }

                var result = _mapper.Map<CategoryAccompanimentDTO>(accompaniment);

                ProcessAccompanimentImageUrls(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea múltiples acompañamientos para una categoría
        /// </summary>
        /// <param name="categoryItemCode">Código de la categoría</param>
        /// <param name="dtos">Lista de acompañamientos a crear</param>
        /// <returns>Lista de acompañamientos creados</returns>
        [HttpPost("{categoryItemCode}")]
        public async Task<ActionResult<IEnumerable<CategoryAccompanimentDTO>>> CreateAccompaniments(
            string categoryItemCode,
            IEnumerable<CategoryAccompanimentForCreationDTO> dtos)
        {
            try
            {
                if (!dtos.Any())
                {
                    return BadRequest("La lista de acompañamientos no puede estar vacía");
                }

                // Verificar que la categoría existe
                var categoryExists = await _context.ProductCategory
                    .AnyAsync(c => c.CategoryItemCode == categoryItemCode);

                if (!categoryExists)
                {
                    return NotFound($"Categoría con código '{categoryItemCode}' no encontrada");
                }

                // Verificar que todos los productos existen
                var accompanimentCodes = dtos.Select(d => d.AccompanimentItemCode).Distinct().ToList();
                var existingProducts = await _context.Product
                    .Where(p => accompanimentCodes.Contains(p.ItemCode))
                    .Select(p => p.ItemCode)
                    .ToListAsync();

                var missingProducts = accompanimentCodes.Except(existingProducts).ToList();
                if (missingProducts.Any())
                {
                    return BadRequest($"Los siguientes productos no existen: {string.Join(", ", missingProducts)}");
                }

                // Obtener el próximo LineNumber inicial
                var startLineNumber = await GetNextLineNumber(categoryItemCode);

                var accompaniments = new List<CategoryAccompaniment>();
                var lineNumber = startLineNumber;

                foreach (var dto in dtos)
                {
                    var accompaniment = _mapper.Map<CategoryAccompaniment>(dto);
                    accompaniment.CategoryItemCode = categoryItemCode;
                    accompaniment.LineNumber = lineNumber;
                    accompaniments.Add(accompaniment);
                    lineNumber++;
                }

                _context.CategoryAccompaniments.AddRange(accompaniments);
                await _context.SaveChangesAsync();

                // Obtener los acompañamientos creados con sus relaciones
                var createdAccompaniments = await _context.CategoryAccompaniments
                    .Include(a => a.AccompanimentProduct)
                    .Include(a => a.Category)
                    .Where(a => a.CategoryItemCode == categoryItemCode &&
                               a.LineNumber >= startLineNumber &&
                               a.LineNumber < startLineNumber + dtos.Count())
                    .OrderBy(a => a.LineNumber)
                    .ToListAsync();

                var result = _mapper.Map<IEnumerable<CategoryAccompanimentDTO>>(createdAccompaniments);

                return CreatedAtAction(
                    nameof(GetCategoryAccompaniments),
                    new { categoryItemCode },
                    result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un solo acompañamiento para una categoría
        /// </summary>
        /// <param name="categoryItemCode">Código de la categoría</param>
        /// <param name="dto">Datos del acompañamiento a crear</param>
        /// <returns>Acompañamiento creado</returns>
        [HttpPost("{categoryItemCode}/single")]
        public async Task<ActionResult<CategoryAccompanimentDTO>> CreateSingleAccompaniment(
            string categoryItemCode,
            CategoryAccompanimentForCreationDTO dto)
        {
            try
            {
                // Verificar que la categoría existe
                var categoryExists = await _context.ProductCategory
                    .AnyAsync(c => c.CategoryItemCode == categoryItemCode);

                if (!categoryExists)
                {
                    return NotFound($"Categoría con código '{categoryItemCode}' no encontrada");
                }

                // Verificar que el producto de acompañamiento existe
                var accompanimentProductExists = await _context.Product
                    .AnyAsync(p => p.ItemCode == dto.AccompanimentItemCode);

                if (!accompanimentProductExists)
                {
                    return BadRequest($"Producto de acompañamiento '{dto.AccompanimentItemCode}' no encontrado");
                }

                // Generar el próximo LineNumber
                var nextLineNumber = await GetNextLineNumber(categoryItemCode);

                // Crear la entidad
                var accompaniment = _mapper.Map<CategoryAccompaniment>(dto);
                accompaniment.CategoryItemCode = categoryItemCode;
                accompaniment.LineNumber = nextLineNumber;

                _context.CategoryAccompaniments.Add(accompaniment);
                await _context.SaveChangesAsync();

                // Obtener el acompañamiento creado con sus relaciones
                var createdAccompaniment = await _context.CategoryAccompaniments
                    .Include(a => a.AccompanimentProduct)
                    .Include(a => a.Category)
                    .FirstOrDefaultAsync(a => a.CategoryItemCode == categoryItemCode && a.LineNumber == nextLineNumber);

                var result = _mapper.Map<CategoryAccompanimentDTO>(createdAccompaniment);

                return CreatedAtAction(
                    nameof(GetAccompaniment),
                    new { categoryItemCode, lineNumber = nextLineNumber },
                    result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza múltiples acompañamientos de una categoría
        /// </summary>
        /// <param name="categoryItemCode">Código de la categoría</param>
        /// <param name="updates">Lista de actualizaciones con LineNumber y datos</param>
        /// <returns>Lista de acompañamientos actualizados</returns>
        [HttpPut("{categoryItemCode}")]
        public async Task<ActionResult<IEnumerable<CategoryAccompanimentDTO>>> UpdateAccompaniments(
            string categoryItemCode,
            IEnumerable<CategoryAccompanimentUpdateBatchDTO> updates)
        {
            try
            {
                if (!updates.Any())
                {
                    return BadRequest("La lista de actualizaciones no puede estar vacía");
                }

                var lineNumbers = updates.Select(u => u.LineNumber).ToList();

                // Obtener todos los acompañamientos a actualizar
                var accompaniments = await _context.CategoryAccompaniments
                    .Where(a => a.CategoryItemCode == categoryItemCode && lineNumbers.Contains(a.LineNumber))
                    .ToListAsync();

                if (accompaniments.Count != updates.Count())
                {
                    var foundLineNumbers = accompaniments.Select(a => a.LineNumber).ToList();
                    var missingLineNumbers = lineNumbers.Except(foundLineNumbers).ToList();
                    return BadRequest($"Los siguientes números de línea no existen: {string.Join(", ", missingLineNumbers)}");
                }

                // Aplicar las actualizaciones
                foreach (var update in updates)
                {
                    var accompaniment = accompaniments.First(a => a.LineNumber == update.LineNumber);
                    _mapper.Map(update.UpdateData, accompaniment);
                }

                await _context.SaveChangesAsync();

                // Obtener los acompañamientos actualizados con sus relaciones
                var updatedAccompaniments = await _context.CategoryAccompaniments
                    .Include(a => a.AccompanimentProduct)
                    .Include(a => a.Category)
                    .Where(a => a.CategoryItemCode == categoryItemCode && lineNumbers.Contains(a.LineNumber))
                    .OrderBy(a => a.LineNumber)
                    .ToListAsync();

                var result = _mapper.Map<IEnumerable<CategoryAccompanimentDTO>>(updatedAccompaniments);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza un acompañamiento existente
        /// </summary>
        /// <param name="categoryItemCode">Código de la categoría</param>
        /// <param name="lineNumber">Número de línea del acompañamiento</param>
        /// <param name="dto">Datos actualizados</param>
        /// <returns>Acompañamiento actualizado</returns>
        [HttpPut("{categoryItemCode}/{lineNumber:int}")]
        public async Task<ActionResult<CategoryAccompanimentDTO>> UpdateSingleAccompaniment(
            string categoryItemCode,
            int lineNumber,
            CategoryAccompanimentForUpdateDTO dto)
        {
            try
            {
                var accompaniment = await _context.CategoryAccompaniments
                    .FirstOrDefaultAsync(a => a.CategoryItemCode == categoryItemCode && a.LineNumber == lineNumber);

                if (accompaniment == null)
                {
                    return NotFound($"Acompañamiento no encontrado para categoría '{categoryItemCode}' y línea {lineNumber}");
                }

                // Mapear los cambios
                _mapper.Map(dto, accompaniment);

                await _context.SaveChangesAsync();

                // Obtener el acompañamiento actualizado con sus relaciones
                var updatedAccompaniment = await _context.CategoryAccompaniments
                    .Include(a => a.AccompanimentProduct)
                    .Include(a => a.Category)
                    .FirstOrDefaultAsync(a => a.CategoryItemCode == categoryItemCode && a.LineNumber == lineNumber);

                var result = _mapper.Map<CategoryAccompanimentDTO>(updatedAccompaniment);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina un acompañamiento
        /// </summary>
        /// <param name="categoryItemCode">Código de la categoría</param>
        /// <param name="lineNumber">Número de línea del acompañamiento</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{categoryItemCode}/{lineNumber:int}")]
        public async Task<IActionResult> DeleteAccompaniment(string categoryItemCode, int lineNumber)
        {
            try
            {
                var accompaniment = await _context.CategoryAccompaniments
                    .FirstOrDefaultAsync(a => a.CategoryItemCode == categoryItemCode && a.LineNumber == lineNumber);

                if (accompaniment == null)
                {
                    return NotFound($"Acompañamiento no encontrado para categoría '{categoryItemCode}' y línea {lineNumber}");
                }

                _context.CategoryAccompaniments.Remove(accompaniment);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina todos los acompañamientos de una categoría
        /// </summary>
        /// <param name="categoryItemCode">Código de la categoría</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{categoryItemCode}")]
        public async Task<IActionResult> DeleteAllCategoryAccompaniments(string categoryItemCode)
        {
            try
            {
                var accompaniments = await _context.CategoryAccompaniments
                    .Where(a => a.CategoryItemCode == categoryItemCode)
                    .ToListAsync();

                if (!accompaniments.Any())
                {
                    return NotFound($"No se encontraron acompañamientos para la categoría '{categoryItemCode}'");
                }

                _context.CategoryAccompaniments.RemoveRange(accompaniments);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene productos disponibles para usar como acompañamientos
        /// </summary>
        /// <returns>Lista de productos disponibles</returns>
        [HttpGet("available-products")]
        public async Task<ActionResult<IEnumerable<object>>> GetAvailableProducts()
        {
            try
            {
                var products = await _context.Product
                    .Where(p => p.Available == "Y" && p.Enabled == "Y" && p.SellItem == "Y")
                    .Select(p => new
                    {
                        p.ItemCode,
                        p.ItemName,
                        p.Price,
                        p.ImageUrl
                    })
                    .OrderBy(p => p.ItemName)
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene el próximo número de línea disponible para una categoría
        /// </summary>
        /// <param name="categoryItemCode">Código de la categoría</param>
        /// <returns>Próximo número de línea</returns>
        private async Task<int> GetNextLineNumber(string categoryItemCode)
        {
            var maxLineNumber = await _context.CategoryAccompaniments
            .Where(a => a.CategoryItemCode == categoryItemCode)
            .AnyAsync()
            ? await _context.CategoryAccompaniments
                .Where(a => a.CategoryItemCode == categoryItemCode)
                .MaxAsync(a => a.LineNumber)
            : 0;

            return maxLineNumber + 1;
        }

        /// <summary>
        /// Construye una URL pública completa a partir de una ruta relativa
        /// </summary>
        /// <param name="relativePath">Ruta relativa de la imagen</param>
        /// <returns>URL pública completa</returns>
        private string BuildPublicUrl(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            return $"{baseUrl}{relativePath}";
        }

        /// <summary>
        /// Procesa las URLs de imágenes de una categoría y sus acompañamientos
        /// </summary>
        /// <param name="category">Categoría a procesar</param>
        private void ProcessCategoryImageUrls(CategoryWithAccompanimentsDTO category)
        {
            // Procesar URL de imagen de la categoría
            if (!string.IsNullOrEmpty(category.ImageUrl))
            {
                category.ImageUrl = BuildPublicUrl(category.ImageUrl);
            }

            // Procesar URLs de imágenes de los acompañamientos
            if (category.AvailableAccompaniments != null)
            {
                foreach (var accompaniment in category.AvailableAccompaniments)
                {
                    ProcessAccompanimentImageUrls(accompaniment);
                }
            }
        }

        /// <summary>
        /// Procesa las URLs de imágenes de un acompañamiento
        /// </summary>
        /// <param name="accompaniment">Acompañamiento a procesar</param>
        private void ProcessAccompanimentImageUrls(CategoryAccompanimentDTO accompaniment)
        {
            // Procesar URL de imagen del producto acompañamiento
            if (!string.IsNullOrEmpty(accompaniment.AccompanimentImageUrl))
            {
                accompaniment.AccompanimentImageUrl = BuildPublicUrl(accompaniment.AccompanimentImageUrl);
            }
        }
    }
}