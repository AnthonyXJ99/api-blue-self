using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueSelfCheckout.Data;
using BlueSelfCheckout.Models;

namespace BlueSelfCheckout.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTreesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ProductTreesController(ApplicationDBContext context)
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
        /// Procesa un ProductTree para incluir URLs completas de imágenes manteniendo compatibilidad.
        /// </summary>
        /// <param name="productTree">ProductTree a procesar</param>
        /// <returns>ProductTree con ImageUrl procesada</returns>
        private ProductTree ProcessProductTreeWithImages(ProductTree productTree)
        {
            if (productTree == null) return null;

            // ✅ Procesar imagen del ProductTree principal (si tiene)
            // Descomenta estas líneas si ProductTree tiene campo ImageUrl
            /*
            if (!string.IsNullOrEmpty(productTree.ImageUrl))
            {
                productTree.ImageUrl = BuildPublicUrl(productTree.ImageUrl);
            }
            */

            // ✅ Procesar imágenes de ProductTreeItems (si tienen)
            if (productTree.Items1 != null && productTree.Items1.Any())
            {
                foreach (var item in productTree.Items1)
                {
                    // Descomenta estas líneas si ProductTreeItem1 tiene campo ImageUrl
                    
                    if (!string.IsNullOrEmpty(item.ImageUrl))
                    {
                        item.ImageUrl = BuildPublicUrl(item.ImageUrl);
                    }
                    
                }
            }

            return productTree;
        }

        /// <summary>
        /// Procesa una lista de ProductTrees para incluir URLs completas de imágenes.
        /// </summary>
        /// <param name="productTrees">Lista de ProductTrees a procesar</param>
        /// <returns>Lista de ProductTree con URLs construidas dinámicamente</returns>
        private IEnumerable<ProductTree> ProcessProductTreesWithImages(IEnumerable<ProductTree> productTrees)
        {
            return productTrees.Select(ProcessProductTreeWithImages);
        }

        // GET: api/ProductTrees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductTree>>> GetProductTree()
        {
            try
            {
                // Incluimos los ProductTreeItems al obtener el ProductTree
                var productTrees = await _context.ProductTree
                    .Include(pt => pt.Items1)  // Cargar los ProductTreeItems relacionados
                    .ToListAsync();

                // ✅ Procesar ProductTrees con URLs dinámicas manteniendo compatibilidad
                var processedProductTrees = ProcessProductTreesWithImages(productTrees);
                return Ok(processedProductTrees);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al obtener los ProductTrees: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }

        // GET: api/ProductTrees/5
        [HttpGet("{itemCode}")]
        public async Task<ActionResult<ProductTree>> GetProductTree(string itemCode)
        {
            try
            {
                var productTree = await _context.ProductTree
                    .Include(pt => pt.Items1)  // Cargar los ProductTreeItems relacionados
                    .FirstOrDefaultAsync(pt => pt.ItemCode == itemCode);

                if (productTree == null)
                {
                    return NotFound();
                }

                // ✅ Procesar ProductTree con URLs dinámicas manteniendo compatibilidad
                var processedProductTree = ProcessProductTreeWithImages(productTree);
                return Ok(processedProductTree);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al obtener el ProductTree con código {itemCode}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }

        // PUT: api/ProductTrees/5
        [HttpPut("{itemCode}")]
        public async Task<IActionResult> PutProductTree(string itemCode, ProductTree productTree)
        {
            if (itemCode != productTree.ItemCode)
            {
                return BadRequest();
            }

            // 1. Get the existing ProductTree and its associated items
            var existingProductTree = await _context.ProductTree
                .Include(pt => pt.Items1)
                .FirstOrDefaultAsync(pt => pt.ItemCode == itemCode);

            if (existingProductTree == null)
            {
                return NotFound();
            }

            // 2. Update parent entity properties
            _context.Entry(existingProductTree).CurrentValues.SetValues(productTree);

            // 3. Sync child entities (ProductTreeItem1)
            var incomingItemCodes = productTree.Items1.Select(i => new { i.ItemCode, i.LineNumber }).ToHashSet();
            var existingItemCodes = existingProductTree.Items1.Select(i => new { i.ItemCode, i.LineNumber }).ToHashSet();

            // Items to remove
            var itemsToRemove = existingProductTree.Items1
                .Where(e => !incomingItemCodes.Contains(new { e.ItemCode, e.LineNumber }))
                .ToList();
            _context.ProductTreeItem1.RemoveRange(itemsToRemove);

            foreach (var incomingItem in productTree.Items1)
            {
                var existingItem = existingProductTree.Items1
                    .FirstOrDefault(e => e.ItemCode == incomingItem.ItemCode && e.LineNumber == incomingItem.LineNumber);

                if (existingItem != null)
                {
                    // Update existing item
                    _context.Entry(existingItem).CurrentValues.SetValues(incomingItem);
                }
                else
                {
                    // Add new item
                    incomingItem.ProductTreeItemCode = productTree.ItemCode;
                    existingProductTree.Items1.Add(incomingItem);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductTreeExists(itemCode))
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


        // POST: api/ProductTrees
        [HttpPost]
        public async Task<ActionResult<ProductTree>> PostProductTree(ProductTree productTree)
        {
            try
            {
                if (productTree.Items1 != null && productTree.Items1.Any())
                {
                    // Assign a LineNumber to each item before adding
                    int lineNumber = 0;
                    foreach (var item in productTree.Items1)
                    {
                        item.LineNumber = lineNumber++;
                        item.ProductTreeItemCode = productTree.ItemCode;
                    }
                }

                _context.ProductTree.Add(productTree);
                await _context.SaveChangesAsync();

                // ... rest of your code to process and return the result
                var createdProductTree = await _context.ProductTree
                    .Include(pt => pt.Items1)
                    .FirstOrDefaultAsync(pt => pt.ItemCode == productTree.ItemCode);

                var processedProductTree = ProcessProductTreeWithImages(createdProductTree);

                return CreatedAtAction("GetProductTree", new { itemCode = productTree.ItemCode }, processedProductTree);
            }
            catch (DbUpdateException ex)
            {
                if (ProductTreeExists(productTree.ItemCode))
                {
                    return Conflict();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al crear ProductTree.", error = ex.Message });
                }
            }
        }
        // DELETE: api/ProductTrees/5
        [HttpDelete("{itemCode}")]
        public async Task<IActionResult> DeleteProductTree(string itemCode)
        {
            try
            {
                var productTree = await _context.ProductTree
                    .Include(pt => pt.Items1)  // Cargar los ProductTreeItems relacionados
                    .FirstOrDefaultAsync(pt => pt.ItemCode == itemCode);

                if (productTree == null)
                {
                    return NotFound();
                }

                // Si existen ProductTreeItems asociados, eliminarlos también
                if (productTree.Items1 != null && productTree.Items1.Any())
                {
                    _context.ProductTreeItem1.RemoveRange(productTree.Items1);
                }

                _context.ProductTree.Remove(productTree);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al eliminar el ProductTree con código {itemCode}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }

        // GET: api/ProductTrees/{itemCode}/items
        /// <summary>
        /// Obtiene solo los items de un ProductTree específico.
        /// </summary>
        /// <param name="itemCode">Código del ProductTree</param>
        /// <returns>Lista de ProductTreeItems</returns>
        [HttpGet("{itemCode}/items")]
        public async Task<ActionResult<IEnumerable<object>>> GetProductTreeItems(string itemCode)
        {
            try
            {
                var productTreeExists = await _context.ProductTree.AnyAsync(pt => pt.ItemCode == itemCode);
                if (!productTreeExists)
                {
                    return NotFound("ProductTree no encontrado");
                }

                var items = await _context.ProductTreeItem1
                    .Where(pti => pti.ProductTreeItemCode == itemCode)
                    .ToListAsync();

                // ✅ Procesar items con URLs dinámicas (si tienen campo de imagen)
                var processedItems = items.Select(item => {
                    // Descomenta y ajusta según los campos reales de ProductTreeItem1
                    /*
                    if (!string.IsNullOrEmpty(item.ImageUrl))
                    {
                        item.ImageUrl = BuildPublicUrl(item.ImageUrl);
                    }
                    */
                    return item;
                });

                return Ok(processedItems);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al obtener items del ProductTree {itemCode}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }

        private bool ProductTreeExists(string itemCode)
        {
            return _context.ProductTree.Any(e => e.ItemCode == itemCode);
        }
    }
}