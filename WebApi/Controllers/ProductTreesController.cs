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

        // GET: api/ProductTrees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductTree>>> GetProductTree()
        {
            // Incluimos los ProductTreeItems al obtener el ProductTree
            return await _context.ProductTree
                .Include(pt => pt.Items1)  // Cargar los ProductTreeItems relacionados
                .ToListAsync();
        }

        // GET: api/ProductTrees/5
        [HttpGet("{itemCode}")]
        public async Task<ActionResult<ProductTree>> GetProductTree(string itemCode)
        {
            var productTree = await _context.ProductTree
                .Include(pt => pt.Items1)  // Cargar los ProductTreeItems relacionados
                .FirstOrDefaultAsync(pt => pt.ItemCode == itemCode);

            if (productTree == null)
            {
                return NotFound();
            }

            return productTree;
        }

        // PUT: api/ProductTrees/5
        [HttpPut("{itemCode}")]
        public async Task<IActionResult> PutProductTree(string itemCode, ProductTree productTree)
        {
            if (itemCode != productTree.ItemCode)
            {
                return BadRequest();
            }

            // Si los ProductTreeItems están siendo modificados, actualizar también
            _context.Entry(productTree).State = EntityState.Modified;

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
            // Añadir los ProductTreeItems antes de guardar el ProductTree
            if (productTree.Items1 != null && productTree.Items1.Any())
            {
                foreach (var item in productTree.Items1)
                {
                    // Asociamos los Items con el ProductTree
                    item.ProductTreeItemCode = productTree.ItemCode;
                    _context.ProductTreeItem1.Add(item);
                }
            }

            _context.ProductTree.Add(productTree);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductTreeExists(productTree.ItemCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProductTree", new { itemCode = productTree.ItemCode }, productTree);
        }

        // DELETE: api/ProductTrees/5
        [HttpDelete("{itemCode}")]
        public async Task<IActionResult> DeleteProductTree(string itemCode)
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

        private bool ProductTreeExists(string itemCode)
        {
            return _context.ProductTree.Any(e => e.ItemCode == itemCode);
        }
    }
}
