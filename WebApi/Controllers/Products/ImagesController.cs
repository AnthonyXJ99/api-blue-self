using BlueSelfCheckout.WebApi.Models.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueSelfCheckout.Data;
using BlueSelfCheckout.WebApi.Models;

namespace BlueSelfCheckout.WebApi.Controllers.Products
{
    /// <summary>
    /// Controlador unificado para gestionar imágenes: almacenamiento físico y registro en base de datos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly string _imagesFolder;

        /// <summary>
        /// Inicializa una nueva instancia del controlador <see cref="ImagesController"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>
        public ImagesController(ApplicationDBContext context)
        {
            _context = context;
            _imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(_imagesFolder))
                Directory.CreateDirectory(_imagesFolder);
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
        /// Sube una imagen, la guarda físicamente y registra en base de datos.
        /// </summary>
        /// <param name="file">Archivo de imagen a subir.</param>
        /// <param name="imageTitle">Título de la imagen.</param>
        /// <param name="imageType">Tipo de imagen (Logo, Publicidad, Item).</param>
        /// <param name="description">Descripción opcional de la imagen.</param>
        /// <returns>La imagen creada con su URL pública.</returns>
        [HttpPost("upload")]
        public async Task<ActionResult<object>> UploadImage(
            [FromForm] IFormFile file,
            [FromForm] string imageTitle,
            [FromForm] string imageType = "Item",
            [FromForm] string description = null,
            [FromForm] string tag = "",
            [FromForm] string deviceId = null)
        {
            string uniqueFileName = null;
            string filePath = null;

            try
            {
                // Validaciones del archivo
                if (file == null || file.Length == 0)
                    return BadRequest("No se envió ningún archivo.");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                    return BadRequest("Tipo de archivo no permitido.");

                // Validaciones de datos
                if (string.IsNullOrEmpty(imageTitle))
                    return BadRequest("El título de la imagen es requerido.");

                var validTypes = new[] { "Logo", "Publicidad", "Item", "Banner" };
                if (!validTypes.Contains(imageType))
                    return BadRequest("Tipo de imagen no válido. Debe ser: Logo, Publicidad, Item o Banner.");

                // Generar nombre único y código único
                uniqueFileName = $"{Guid.NewGuid()}{ext}";
                var imageCode = $"IMG_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8]}";

                // Guardar archivo físicamente
                filePath = Path.Combine(_imagesFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // ✅ CAMBIO PRINCIPAL: Solo guardar ruta relativa
                var relativePath = $"/images/{uniqueFileName}";

                // Crear objeto Image para base de datos
                var image = new Image
                {
                    ImageCode = imageCode,
                    ImageTitle = imageTitle,
                    ImageType = imageType,
                    Description = description,
                    FileName = uniqueFileName,
                    FilePath = filePath, // Ruta física del servidor
                    PublicUrl = relativePath, // ✅ Ahora es ruta relativa
                    OriginalFileName = file.FileName,
                    FileSize = file.Length,
                    ContentType = file.ContentType,
                    CreatedAt = DateTime.Now,
                    Tags = imageTitle + " " + tag,
                    DeviceCode = deviceId,
                    IsActive = true
                };

                // Guardar en base de datos
                _context.Image.Add(image);
                await _context.SaveChangesAsync();

                // ✅ Respuesta con URL completa construida dinámicamente
                var response = new
                {
                    image.ImageCode,
                    image.ImageTitle,
                    image.ImageType,
                    image.Description,
                    image.FileName,
                    PublicUrl = BuildPublicUrl(image.PublicUrl), // URL completa para el cliente
                    RelativePath = image.PublicUrl, // Ruta relativa
                    image.OriginalFileName,
                    image.FileSize,
                    image.ContentType,
                    image.CreatedAt,
                    image.Tags,
                    image.DeviceCode,
                    image.IsActive
                };

                return CreatedAtAction("GetImage", new { imageCode = image.ImageCode }, response);
            }
            catch (Exception ex)
            {
                // Si hay error, intentar eliminar archivo físico si se creó
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                Console.Error.WriteLine($"Error al subir imagen: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error al procesar la imagen", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un listado con todas las imágenes.
        /// </summary>
        /// <returns>Una lista de imágenes.</returns>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllImages()
        {
            var images = await _context.Image.Where(i => i.IsActive).ToListAsync();

            // ✅ Construir URLs completas dinámicamente
            var response = images.Select(img => new
            {
                img.ImageCode,
                img.ImageTitle,
                img.ImageType,
                img.Description,
                img.FileName,
                PublicUrl = BuildPublicUrl(img.PublicUrl), // URL completa
                RelativePath = img.PublicUrl, // Ruta relativa
                img.OriginalFileName,
                img.FileSize,
                img.ContentType,
                img.CreatedAt,
                img.Tags,
                img.DeviceCode,
                img.IsActive
            });

            return Ok(response);
        }

        /// <summary>
        /// Obtiene imágenes con paginación y filtros.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<object>> GetImages(
            int pageNumber = 1,
            int pageSize = 10,
            string search = null,
            string imageType = null)
        {
            try
            {
                IQueryable<Image> query = _context.Image.Where(i => i.IsActive);

                // Filtro por búsqueda
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(i => i.ImageCode.Contains(search) ||
                                           i.ImageTitle.Contains(search) ||
                                           i.Description.Contains(search) ||
                                           i.Tags.Contains(search));
                }

                // Filtro por tipo de imagen
                if (!string.IsNullOrEmpty(imageType))
                {
                    query = query.Where(i => i.ImageType == imageType);
                }

                var totalCount = await query.CountAsync();
                var images = await query
                    .OrderByDescending(i => i.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // ✅ Construir URLs completas dinámicamente
                var imageResponses = images.Select(img => new
                {
                    img.ImageCode,
                    img.ImageTitle,
                    img.ImageType,
                    img.Description,
                    img.FileName,
                    PublicUrl = BuildPublicUrl(img.PublicUrl), // URL completa
                    RelativePath = img.PublicUrl, // Ruta relativa
                    img.OriginalFileName,
                    img.FileSize,
                    img.ContentType,
                    img.CreatedAt,
                    img.Tags,
                    img.DeviceCode,
                    img.IsActive
                });

                var response = new
                {
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    Data = imageResponses
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al obtener imágenes: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error al obtener imágenes", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene imágenes por tipo específico.
        /// </summary>
        /// <param name="type">Tipo de imagen (Logo, Publicidad, Item).</param>
        /// <returns>Lista de imágenes del tipo especificado.</returns>
        [HttpGet("by-type/{type}")]
        public async Task<ActionResult<IEnumerable<object>>> GetImagesByType(string type)
        {
            var validTypes = new[] { "Logo", "Publicidad", "Item", "Banner" };
            if (!validTypes.Contains(type))
                return BadRequest("Tipo de imagen no válido.");

            var images = await _context.Image
                .Where(i => i.ImageType == type && i.IsActive)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            // ✅ Construir URLs completas dinámicamente
            var response = images.Select(img => new
            {
                img.ImageCode,
                img.ImageTitle,
                img.ImageType,
                img.Description,
                img.FileName,
                PublicUrl = BuildPublicUrl(img.PublicUrl), // URL completa
                RelativePath = img.PublicUrl, // Ruta relativa
                img.OriginalFileName,
                img.FileSize,
                img.ContentType,
                img.CreatedAt,
                img.Tags,
                img.DeviceCode,
                img.IsActive
            });

            return Ok(response);
        }

        /// <summary>
        /// Obtiene una imagen específica por código.
        /// </summary>
        [HttpGet("{imageCode}")]
        public async Task<ActionResult<object>> GetImage(string imageCode)
        {
            var image = await _context.Image
                .FirstOrDefaultAsync(i => i.ImageCode == imageCode && i.IsActive);

            if (image == null)
                return NotFound();

            // ✅ Construir URL completa dinámicamente
            var response = new
            {
                image.ImageCode,
                image.ImageTitle,
                image.ImageType,
                image.Description,
                image.FileName,
                PublicUrl = BuildPublicUrl(image.PublicUrl), // URL completa
                RelativePath = image.PublicUrl, // Ruta relativa
                image.OriginalFileName,
                image.FileSize,
                image.ContentType,
                image.CreatedAt,
                image.Tags,
                image.DeviceCode,
                image.IsActive
            };

            return Ok(response);
        }

        /// <summary>
        /// Actualiza los datos de una imagen existente (sin cambiar el archivo).
        /// </summary>
        [HttpPut("{imageCode}")]
        public async Task<IActionResult> UpdateImage(string imageCode, [FromBody] UpdateImageRequest request)
        {
            try
            {
                var image = await _context.Image.FindAsync(imageCode);
                if (image == null)
                    return NotFound();

                // Actualizar solo los campos permitidos
                if (!string.IsNullOrEmpty(request.ImageTitle))
                    image.ImageTitle = request.ImageTitle;

                if (!string.IsNullOrEmpty(request.ImageType))
                {
                    var validTypes = new[] { "Logo", "Publicidad", "Item", "Banner" };
                    if (!validTypes.Contains(request.ImageType))
                        return BadRequest("Tipo de imagen no válido.");
                    image.ImageType = request.ImageType;
                }

                if (request.Description != null)
                    image.Description = request.Description;
                if (!string.IsNullOrEmpty(request.Tag))
                    image.Tags = request.Tag;
                if (!string.IsNullOrWhiteSpace(request.DeviceCode))
                    image.DeviceCode = request.DeviceCode;

                image.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al actualizar imagen: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error al actualizar imagen", error = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una imagen (archivo físico y registro en BD).
        /// </summary>
        [HttpDelete("{imageCode}")]
        public async Task<IActionResult> DeleteImage(string imageCode)
        {
            try
            {
                var image = await _context.Image.FindAsync(imageCode);
                if (image == null)
                    return NotFound();

                // Eliminar archivo físico
                if (!string.IsNullOrEmpty(image.FilePath) && System.IO.File.Exists(image.FilePath))
                {
                    System.IO.File.Delete(image.FilePath);
                }

                // Eliminar registro de base de datos (soft delete)
                image.IsActive = false;
                image.DeletedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al eliminar imagen: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error al eliminar imagen", error = ex.Message });
            }
        }

        /// <summary>
        /// Lista todas las imágenes físicas disponibles en la carpeta.
        /// </summary>
        [HttpGet("files/list")]
        public IActionResult ListImageFiles()
        {
            var files = Directory.GetFiles(_imagesFolder)
                .Select(f => new
                {
                    fileName = Path.GetFileName(f),
                    relativePath = $"/images/{Path.GetFileName(f)}", // ✅ Ruta relativa
                    publicUrl = BuildPublicUrl($"/images/{Path.GetFileName(f)}"), // URL completa
                    size = new FileInfo(f).Length,
                    createdAt = System.IO.File.GetCreationTime(f)
                })
                .ToList();

            return Ok(files);
        }

        /// <summary>
        /// Verifica si existe una imagen con el código especificado.
        /// </summary>
        private bool ImageExists(string imageCode)
        {
            return _context.Image.Any(e => e.ImageCode == imageCode && e.IsActive);
        }
    }

    /// <summary>
    /// Modelo para actualizar datos de imagen.
    /// </summary>
    public class UpdateImageRequest
    {
        public string ImageTitle { get; set; }
        public string ImageType { get; set; }
        public string Description { get; set; }
        public string Tag { get; set; }
        public string? DeviceCode { get; set; }
    }
}