using Microsoft.AspNetCore.Mvc;

namespace BlueSelfCheckout.WebApi.Controllers.Images
{
    /// <summary>
    /// Controlador para gestionar operaciones relacionadas con imágenes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ImageFileController : ControllerBase
    {
        private readonly string _imagesFolder;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ImageFileController"/>.
        /// </summary>
        public ImageFileController()
        {
            _imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(_imagesFolder))
                Directory.CreateDirectory(_imagesFolder);
        }

        /// <summary>
        /// Sube una imagen y devuelve su URL pública.
        /// </summary>
        /// <param name="file">Imagen a subir (form-data, key: file).</param>
        /// <returns>URL pública de la imagen subida.</returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se envió ningún archivo.");

            // Validar extensiones permitidas
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                return BadRequest("Tipo de archivo no permitido.");

            // Generar nombre único para evitar reemplazos
            var uniqueFileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(_imagesFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Construir URL pública
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var publicUrl = $"{baseUrl}/images/{uniqueFileName}";

            return Ok(new { url = publicUrl });
        }

        /// <summary>
        /// Lista todas las imágenes disponibles.
        /// </summary>
        /// <returns>Lista de URLs de las imágenes.</returns>
        [HttpGet("list")]
        public IActionResult ListImages()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var files = Directory.GetFiles(_imagesFolder)
                .Select(f => new
                {
                    fileName = Path.GetFileName(f),
                    url = $"{baseUrl}/images/{Path.GetFileName(f)}"
                })
                .ToList();

            return Ok(files);
        }

        /// <summary>
        /// Elimina una imagen por nombre de archivo.
        /// </summary>
        /// <param name="fileName">Nombre del archivo a eliminar.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpDelete("delete/{fileName}")]
        public IActionResult DeleteImage(string fileName)
        {
            // Seguridad: prevenir path traversal
            if (string.IsNullOrEmpty(fileName) || fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                return BadRequest("Nombre de archivo inválido.");

            var filePath = Path.Combine(_imagesFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("La imagen no existe.");

            System.IO.File.Delete(filePath);
            return Ok(new { message = "Imagen eliminada correctamente." });
        }
    }
}
